using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Tenant;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// Tenant management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class TenantController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TenantController> _logger;
        private readonly IValidator<CreateTenantdto> _createValidator;
        private readonly IValidator<UpdateTenantDto> _updateValidator;

        public TenantController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<TenantController> logger,
            IValidator<CreateTenantdto> createValidator,
            IValidator<UpdateTenantDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all tenant with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="status">Tenant status filter</param>
        
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of customers</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<TenantDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<TenantDto>>> GetTenants(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? status = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching tenants for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (tenants, totalCount) = await _unitOfWork.Tenants.GetTenantPagedAsync(
                    tenantId, searchTerm, status, pageNumber, pageSize);

                var tenantDtos = _mapper.Map<List<TenantDto>>(tenants);

                var response = new PagedResponse<TenantDto>(tenantDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} tenants out of {TotalCount} for tenant {TenantId}",
                    tenantDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching tenants for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get tenant by ID
        /// </summary>
        /// <param name="id">Tenant ID</param>
        /// <returns>Tenant details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<TenantDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<TenantDto>>> GetTenantById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching tenant with ID: {TenantId}", id);

                var tenant = await _unitOfWork.Tenants.GetByIdAsync(id);

                if (tenant == null)
                {
                    _logger.LogWarning("Tenant {TenantId} not found", id);
                    return NotFound(new ApiResponse<object>($"Tenant with ID {id} not found"));
                }

                var tenantDto = _mapper.Map<TenantDto>(tenant);

                _logger.LogInformation("Successfully retrieved tenant {TenantId}", id);

                return Ok(new ApiResponse<TenantDto>(tenantDto, "Tenant retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching tenant {TenantId}", id);
                throw;
            }
        }

        /// <summary>
        /// Get tenant by tenant code
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="tenantCode">Tenant code</param>
        /// <returns>Tenant details</returns>
        [HttpGet("by-code/{tenantCode}")]
        [ProducesResponseType(typeof(ApiResponse<TenantDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<TenantDto>>> GetTenantByCode(
            [FromQuery] Guid tenantId,
            string tenantCode)
        {
            try
            {
                _logger.LogInformation("Fetching tenant with code {TenantCode} for tenant {TenantId}",
                    tenantCode, tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var tenant = await _unitOfWork.Tenants.GetByTenantCodeAsync(tenantId, tenantCode);

                if (tenant == null)
                {
                    _logger.LogWarning("Tenant with code {TenantCode} not found for tenant {TenantId}",
                        tenantCode, tenantId);
                    return NotFound(new ApiResponse<object>($"Tenant with code {tenantCode} not found"));
                }

                var tenantDto = _mapper.Map<TenantDto>(tenant);

                _logger.LogInformation("Successfully retrieved tenant with code {TenantCode}", tenantCode);

                return Ok(new ApiResponse<TenantDto>(tenantDto, "Tenant retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching tenant with code {TenantCode}", tenantCode);
                throw;
            }
        }

        /// <summary>
        /// Create a new tenant
        /// </summary>
        /// <param name="createTenantDto">Tenant creation data</param>
        /// <returns>Created tenant details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<TenantDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<TenantDto>>> CreateTenant([FromBody] CreateTenantdto createTenantDto)
        {
            try
            {
                _logger.LogInformation("Creating new tenant with code {TenantCode} for tenant {TenantId}",
                    createTenantDto.TenantCode, createTenantDto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createTenantDto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for tenant creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Check if tenant code is unique
                var isUnique = await _unitOfWork.Tenants.IsTenantCodeUniqueAsync(
                    createTenantDto.TenantId, createTenantDto.TenantCode);

                if (!isUnique)
                {
                    _logger.LogWarning("Tenant code {TenantCode} already exists for tenant {TenantId}",
                        createTenantDto.TenantCode, createTenantDto.TenantId);
                    return BadRequest(new ApiResponse<object>("Tenant code already exists"));
                }

                // Map DTO to entity
                var tenant = _mapper.Map<Tenant>(createTenantDto);
                tenant.TenantId = Guid.NewGuid();
                tenant.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.Tenants.AddAsync(tenant);
                await _unitOfWork.SaveChangesAsync();

                var tenantDto = _mapper.Map<TenantDto>(tenant);

                _logger.LogInformation("Successfully created tenant {TenantId} with code {TenantCode}",
                    tenant.TenantId, tenant.TenantCode);

                return CreatedAtAction(
                    nameof(GetTenantById),
                    new { id = tenant.TenantId },
                    new ApiResponse<TenantDto>(tenantDto, "Tenant created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating tenant");
                throw;
            }
        }

        /// <summary>
        /// Update an existing tenant
        /// </summary>
        /// <param name="id">Tenant ID</param>
        /// <param name="updateTenantDto">Tenant update data</param>
        /// <returns>Updated tenant details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<TenantDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<TenantDto>>> UpdateTenant(
            Guid id,
            [FromBody] UpdateTenantDto updateTenantDto)
        {
            try
            {
                _logger.LogInformation("Updating tenant {TenantId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateTenantDto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for tenant update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing tenant
                var existingTenant = await _unitOfWork.Tenants.GetByIdAsync(id);
                if (existingTenant == null)
                {
                    _logger.LogWarning("Tenant {TenantId} not found", id);
                    return NotFound(new ApiResponse<object>($"Tenant with ID {id} not found"));
                }

                // Update only provided fields
                if (!string.IsNullOrWhiteSpace(updateTenantDto.TenantName))
                    existingTenant.TenantName = updateTenantDto.TenantName;

                
                if (!string.IsNullOrWhiteSpace(updateTenantDto.ContactEmail))
                    existingTenant.ContactEmail = updateTenantDto.ContactEmail;

                if (!string.IsNullOrWhiteSpace(updateTenantDto.ContactPhone))
                    existingTenant.ContactPhone = updateTenantDto.ContactPhone;

                if (!string.IsNullOrWhiteSpace(updateTenantDto.Mobile))
                    existingTenant.Mobile = updateTenantDto.Mobile;

                
                if (!string.IsNullOrWhiteSpace(updateTenantDto.Address))
                    existingTenant.Address = updateTenantDto.Address;

                if (!string.IsNullOrWhiteSpace(updateTenantDto.City))
                    existingTenant.City = updateTenantDto.City;

                if (!string.IsNullOrWhiteSpace(updateTenantDto.State))
                    existingTenant.State = updateTenantDto.State;

                if (!string.IsNullOrWhiteSpace(updateTenantDto.Country))
                    existingTenant.Country = updateTenantDto.Country;

                if (!string.IsNullOrWhiteSpace(updateTenantDto.PostalCode))
                    existingTenant.PostalCode = updateTenantDto.PostalCode;

                if (!string.IsNullOrWhiteSpace(updateTenantDto.TenantStatus))
                    existingTenant.TenantStatus = updateTenantDto.TenantStatus;

                if (updateTenantDto.IsActive.HasValue)
                    existingTenant.IsActive = updateTenantDto.IsActive.Value;


                existingTenant.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Tenants.Update(existingTenant);
                await _unitOfWork.SaveChangesAsync();

                var tenantDto = _mapper.Map<TenantDto>(existingTenant);

                _logger.LogInformation("Successfully updated Tenant {TenantId}", id);

                return Ok(new ApiResponse<TenantDto>(tenantDto, "Tenant updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating tenant {TenantId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a tenant (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteTenant(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting tenant {TenantId}", id);

                var tenant = await _unitOfWork.Tenants.GetByIdAsync(id);
                if (tenant == null)
                {
                    _logger.LogWarning("Tenant {TenantId} not found", id);
                    return NotFound(new ApiResponse<object>($"Tenant with ID {id} not found"));
                }

                // Soft delete
                tenant.IsActive = false;
                tenant.TenantStatus = "Inactive";
                tenant.ModifiedDate = DateTime.UtcNow;


                _unitOfWork.Tenants.Update(tenant);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted tenant {TenantId}", id);

                return Ok(new ApiResponse<object>(null, "Tenant deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting tenant {TenantId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search tenant by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching tenant</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<TenantDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<TenantDto>>>> SearchTenant(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching tenants for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var tenant = await _unitOfWork.Tenants.SearchTenantsAsync(tenantId, searchTerm);
                var tenantDtos = _mapper.Map<List<TenantDto>>(tenant);

                _logger.LogInformation("Found {Count} tenants matching search term", tenantDtos.Count);

                return Ok(new ApiResponse<List<TenantDto>>(tenantDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching tenants");
                throw;
            }
        }

        /// <summary>
        /// Get active tenant for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active tenants</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<TenantDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<TenantDto>>>> GetActiveTenant([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active tenants for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var tenant = await _unitOfWork.Tenants.GetActiveTenantsAsync(tenantId);
                var tenantDtos = _mapper.Map<List<TenantDto>>(tenant);

                _logger.LogInformation("Successfully retrieved {Count} active tenant", tenantDtos.Count);

                return Ok(new ApiResponse<List<TenantDto>>(tenantDtos, "Active tenant retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active tenant for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}