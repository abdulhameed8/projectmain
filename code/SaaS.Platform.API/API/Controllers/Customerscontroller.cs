using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Customer;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// Customer management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class CustomersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomersController> _logger;
        private readonly IValidator<CreateCustomerDto> _createValidator;
        private readonly IValidator<UpdateCustomerDto> _updateValidator;

        public CustomersController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CustomersController> logger,
            IValidator<CreateCustomerDto> createValidator,
            IValidator<UpdateCustomerDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all customers with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="status">Customer status filter</param>
        /// <param name="segment">Customer segment filter</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of customers</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<CustomerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<CustomerDto>>> GetCustomers(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? status = null,
            [FromQuery] string? segment = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching customers for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
                    tenantId, pageNumber, pageSize);

                if (tenantId == Guid.Empty)
                {
                    _logger.LogWarning("Invalid tenant ID provided");
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                {
                    _logger.LogWarning("Invalid pagination parameters: PageNumber={PageNumber}, PageSize={PageSize}",
                        pageNumber, pageSize);
                    return BadRequest(new ApiResponse<object>("Invalid pagination parameters"));
                }

                var (customers, totalCount) = await _unitOfWork.Customers.GetCustomersPagedAsync(
                    tenantId, searchTerm, status, segment, pageNumber, pageSize);

                var customerDtos = _mapper.Map<List<CustomerDto>>(customers);

                var response = new PagedResponse<CustomerDto>(customerDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} customers out of {TotalCount} for tenant {TenantId}",
                    customerDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching customers for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get customer by ID
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>Customer details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> GetCustomerById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching customer with ID: {CustomerId}", id);

                var customer = await _unitOfWork.Customers.GetByIdAsync(id);

                if (customer == null)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found", id);
                    return NotFound(new ApiResponse<object>($"Customer with ID {id} not found"));
                }

                var customerDto = _mapper.Map<CustomerDto>(customer);

                _logger.LogInformation("Successfully retrieved customer {CustomerId}", id);

                return Ok(new ApiResponse<CustomerDto>(customerDto, "Customer retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching customer {CustomerId}", id);
                throw;
            }
        }

        /// <summary>
        /// Get customer by customer code
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="customerCode">Customer code</param>
        /// <returns>Customer details</returns>
        [HttpGet("by-code/{customerCode}")]
        [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> GetCustomerByCode(
            [FromQuery] Guid tenantId,
            string customerCode)
        {
            try
            {
                _logger.LogInformation("Fetching customer with code {CustomerCode} for tenant {TenantId}",
                    customerCode, tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var customer = await _unitOfWork.Customers.GetByCustomerCodeAsync(tenantId, customerCode);

                if (customer == null)
                {
                    _logger.LogWarning("Customer with code {CustomerCode} not found for tenant {TenantId}",
                        customerCode, tenantId);
                    return NotFound(new ApiResponse<object>($"Customer with code {customerCode} not found"));
                }

                var customerDto = _mapper.Map<CustomerDto>(customer);

                _logger.LogInformation("Successfully retrieved customer with code {CustomerCode}", customerCode);

                return Ok(new ApiResponse<CustomerDto>(customerDto, "Customer retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching customer with code {CustomerCode}", customerCode);
                throw;
            }
        }

        /// <summary>
        /// Create a new customer
        /// </summary>
        /// <param name="createCustomerDto">Customer creation data</param>
        /// <returns>Created customer details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> CreateCustomer([FromBody] CreateCustomerDto createCustomerDto)
        {
            try
            {
                _logger.LogInformation("Creating new customer with code {CustomerCode} for tenant {TenantId}",
                    createCustomerDto.CustomerCode, createCustomerDto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createCustomerDto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Check if customer code is unique
                var isUnique = await _unitOfWork.Customers.IsCustomerCodeUniqueAsync(
                    createCustomerDto.TenantId, createCustomerDto.CustomerCode);

                if (!isUnique)
                {
                    _logger.LogWarning("Customer code {CustomerCode} already exists for tenant {TenantId}",
                        createCustomerDto.CustomerCode, createCustomerDto.TenantId);
                    return BadRequest(new ApiResponse<object>("Customer code already exists"));
                }

                // Map DTO to entity
                var customer = _mapper.Map<Customer>(createCustomerDto);
                customer.CustomerId = Guid.NewGuid();
                customer.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.Customers.AddAsync(customer);
                await _unitOfWork.SaveChangesAsync();

                var customerDto = _mapper.Map<CustomerDto>(customer);

                _logger.LogInformation("Successfully created customer {CustomerId} with code {CustomerCode}",
                    customer.CustomerId, customer.CustomerCode);

                return CreatedAtAction(
                    nameof(GetCustomerById),
                    new { id = customer.CustomerId },
                    new ApiResponse<CustomerDto>(customerDto, "Customer created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating customer");
                throw;
            }
        }

        /// <summary>
        /// Update an existing customer
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <param name="updateCustomerDto">Customer update data</param>
        /// <returns>Updated customer details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CustomerDto>>> UpdateCustomer(
            Guid id,
            [FromBody] UpdateCustomerDto updateCustomerDto)
        {
            try
            {
                _logger.LogInformation("Updating customer {CustomerId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateCustomerDto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing customer
                var existingCustomer = await _unitOfWork.Customers.GetByIdAsync(id);
                if (existingCustomer == null)
                {
                    _logger.LogWarning("Customer {CustomerId} not found", id);
                    return NotFound(new ApiResponse<object>($"Customer with ID {id} not found"));
                }

                // Update only provided fields
                if (!string.IsNullOrWhiteSpace(updateCustomerDto.FirstName))
                    existingCustomer.FirstName = updateCustomerDto.FirstName;

                if (!string.IsNullOrWhiteSpace(updateCustomerDto.LastName))
                    existingCustomer.LastName = updateCustomerDto.LastName;

                if (!string.IsNullOrWhiteSpace(updateCustomerDto.CompanyName))
                    existingCustomer.CompanyName = updateCustomerDto.CompanyName;

                if (!string.IsNullOrWhiteSpace(updateCustomerDto.Email))
                    existingCustomer.Email = updateCustomerDto.Email;

                if (!string.IsNullOrWhiteSpace(updateCustomerDto.Phone))
                    existingCustomer.Phone = updateCustomerDto.Phone;

                if (!string.IsNullOrWhiteSpace(updateCustomerDto.Mobile))
                    existingCustomer.Mobile = updateCustomerDto.Mobile;

                if (updateCustomerDto.DateOfBirth.HasValue)
                    existingCustomer.DateOfBirth = updateCustomerDto.DateOfBirth;

                if (!string.IsNullOrWhiteSpace(updateCustomerDto.Gender))
                    existingCustomer.Gender = updateCustomerDto.Gender;

                if (!string.IsNullOrWhiteSpace(updateCustomerDto.Address))
                    existingCustomer.Address = updateCustomerDto.Address;

                if (!string.IsNullOrWhiteSpace(updateCustomerDto.City))
                    existingCustomer.City = updateCustomerDto.City;

                if (!string.IsNullOrWhiteSpace(updateCustomerDto.State))
                    existingCustomer.State = updateCustomerDto.State;

                if (!string.IsNullOrWhiteSpace(updateCustomerDto.Country))
                    existingCustomer.Country = updateCustomerDto.Country;

                if (!string.IsNullOrWhiteSpace(updateCustomerDto.PostalCode))
                    existingCustomer.PostalCode = updateCustomerDto.PostalCode;

                if (!string.IsNullOrWhiteSpace(updateCustomerDto.CustomerStatus))
                    existingCustomer.CustomerStatus = updateCustomerDto.CustomerStatus;

                if (updateCustomerDto.AssignedUserId.HasValue)
                    existingCustomer.AssignedUserId = updateCustomerDto.AssignedUserId;

                if (updateCustomerDto.CreditLimit.HasValue)
                    existingCustomer.CreditLimit = updateCustomerDto.CreditLimit.Value;

                if (updateCustomerDto.CreditScore.HasValue)
                    existingCustomer.CreditScore = updateCustomerDto.CreditScore;

                if (updateCustomerDto.IsActive.HasValue)
                    existingCustomer.IsActive = updateCustomerDto.IsActive.Value;

                existingCustomer.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Customers.Update(existingCustomer);
                await _unitOfWork.SaveChangesAsync();

                var customerDto = _mapper.Map<CustomerDto>(existingCustomer);

                _logger.LogInformation("Successfully updated customer {CustomerId}", id);

                return Ok(new ApiResponse<CustomerDto>(customerDto, "Customer updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating customer {CustomerId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a customer (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteCustomer(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting customer {CustomerId}", id);

                var customer = await _unitOfWork.Customers.GetByIdAsync(id);
                if (customer == null)
                {
                    _logger.LogWarning("Customer {CustomerId} not found", id);
                    return NotFound(new ApiResponse<object>($"Customer with ID {id} not found"));
                }

                // Soft delete
                customer.IsActive = false;
                customer.CustomerStatus = "Inactive";
                customer.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Customers.Update(customer);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted customer {CustomerId}", id);

                return Ok(new ApiResponse<object>(null, "Customer deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting customer {CustomerId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search customers by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching customers</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<CustomerDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<CustomerDto>>>> SearchCustomers(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching customers for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var customers = await _unitOfWork.Customers.SearchCustomersAsync(tenantId, searchTerm);
                var customerDtos = _mapper.Map<List<CustomerDto>>(customers);

                _logger.LogInformation("Found {Count} customers matching search term", customerDtos.Count);

                return Ok(new ApiResponse<List<CustomerDto>>(customerDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching customers");
                throw;
            }
        }

        /// <summary>
        /// Get active customers for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active customers</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<CustomerDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<CustomerDto>>>> GetActiveCustomers([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active customers for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var customers = await _unitOfWork.Customers.GetActiveCustomersAsync(tenantId);
                var customerDtos = _mapper.Map<List<CustomerDto>>(customers);

                _logger.LogInformation("Successfully retrieved {Count} active customers", customerDtos.Count);

                return Ok(new ApiResponse<List<CustomerDto>>(customerDtos, "Active customers retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active customers for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}