using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// PaymentMethod management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentMethodsController> _logger;
        private readonly IValidator<CreatePaymentMethodsdto> _createValidator;
        private readonly IValidator<UpdatePaymentMethodsdto> _updateValidator;

        public PaymentMethodsController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<PaymentMethodsController> logger,
            IValidator<CreatePaymentMethodsdto> createValidator,
            IValidator<UpdatePaymentMethodsdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all PaymentMethods with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of PaymentMethods</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<PaymentMethodsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<PaymentMethodsdto>>> GetPaymentMethods(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching PaymentMethods for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (paymentMethod, totalCount) = await _unitOfWork.PaymentMethods.GetPaymentMethodsPagedAsync(
                    tenantId, searchTerm, pageNumber, pageSize);

                var paymentMethodDtos = _mapper.Map<List<PaymentMethodsdto>>(paymentMethod);

                var response = new PagedResponse<PaymentMethodsdto>(paymentMethodDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} PaymentMethods out of {TotalCount} for tenant {TenantId}",
                    paymentMethodDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching PaymentMethods for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// </summary>
        /// <param name="id">PaymentMethods ID</param>
        /// <returns>Roles details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PaymentMethodsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]

        public async Task<ActionResult<ApiResponse<PaymentMethodsdto>>> GetPaymentMethodsById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching PaymentMethods with ID: {PaymentMethodsId}", id);

                var paymentMethod = await _unitOfWork.PaymentMethods.GetByIdAsync(id);

                if (paymentMethod == null)
                {
                    _logger.LogWarning("PaymentMethods with ID {PaymentMethodsId} not found", id);
                    return NotFound(new ApiResponse<object>($"PaymentMethods with ID {id} not found"));
                }

                var paymentMethodDto = _mapper.Map<PaymentMethodsdto>(paymentMethod);

                _logger.LogInformation("Successfully retrieved PaymentMethods {PaymentMethodId}", id);

                return Ok(new ApiResponse<PaymentMethodsdto>(paymentMethodDto, "PaymentMethods retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching PaymentMethods {PaymentMethodId}", id);
                throw;
            }
        }

        /// <summary>
        /// Create a new PaymentMethods
        /// </summary>
        /// <param name="createPaymentMethodsdto">Roles creation data</param>
        /// <returns>Created PaymentMethods details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PaymentMethodsdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PaymentMethodsdto>>> CreatePaymentMethods([FromBody] CreatePaymentMethodsdto createPaymentMethodsdto)
        {
            try
            {
                _logger.LogInformation("Creating new PaymentMethods  for tenant {TenantId}",
                     createPaymentMethodsdto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createPaymentMethodsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for PaymentMethods creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }


                // Map DTO to entity
                var paymentMethods = _mapper.Map<PaymentMethods>(createPaymentMethodsdto);
                paymentMethods.PaymentMethodId = Guid.NewGuid();
                paymentMethods.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.PaymentMethods.AddAsync(paymentMethods);
                await _unitOfWork.SaveChangesAsync();

                var paymentMethodDto = _mapper.Map<PaymentMethodsdto>(paymentMethods);

                _logger.LogInformation("Successfully created PaymentMethods {PaymentMethodId} )",
                    paymentMethods.PaymentMethodId);

                return CreatedAtAction(
                    nameof(GetPaymentMethodsById),
                    new { id = paymentMethods.PaymentMethodId },
                    new ApiResponse<PaymentMethodsdto>(paymentMethodDto, "PaymentMethods created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating roles");
                throw;
            }
        }

        /// <summary>
        /// Update an existing PaymentMethods
        /// </summary>
        /// <param name="id">PaymentMethods ID</param>
        /// <param name="updatePaymentMethodsDto">PaymentMethods update data</param>
        /// <returns>Updated PaymentMethods details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PaymentMethodsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PaymentMethodsdto>>> UpdatePaymentMethods(
            Guid id,
            [FromBody] UpdatePaymentMethodsdto updatePaymentMethodsdto)
        {
            try
            {
                _logger.LogInformation("Updating PaymentMethods  {PaymentMethodId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updatePaymentMethodsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for PaymentMethods update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing customer
                var existingPaymentMethods = await _unitOfWork.PaymentMethods.GetByIdAsync(id);
                if (existingPaymentMethods == null)
                {
                    _logger.LogWarning("PaymentMethods {PaymentMethodId} not found", id);
                    return NotFound(new ApiResponse<object>($"PaymentMethods with ID {id} not found"));
                }

                // Update only provided fields

                if (updatePaymentMethodsdto.IsActive.HasValue)
                    existingPaymentMethods.IsActive = updatePaymentMethodsdto.IsActive.Value;


                _unitOfWork.PaymentMethods.Update(existingPaymentMethods);
                await _unitOfWork.SaveChangesAsync();

                var PaymentMethodsDto = _mapper.Map<PaymentMethodsdto>(existingPaymentMethods);

                _logger.LogInformation("Successfully updated PaymentMethods {PaymentMethodId}", id);

                return Ok(new ApiResponse<PaymentMethodsdto>(PaymentMethodsDto, "PaymentMethods updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating PaymentMethods {PaymentMethodId}", id);
                throw;
            }
        }
        /// <summary>
        /// Delete a PaymentMethods (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">PaymentMethods ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeletePaymentMethods(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting PaymentMethods {PaymentMethodId}", id);

                var paymentMethods = await _unitOfWork.PaymentMethods.GetByIdAsync(id);
                if (paymentMethods == null)
                {
                    _logger.LogWarning("PaymentMethods {PaymentMethodId} PaymentMethods not found", id);
                    return NotFound(new ApiResponse<object>($"PaymentMethods with ID {id} not found"));
                }

                // Soft delete
                paymentMethods.IsActive = false;

                _unitOfWork.PaymentMethods.Update(paymentMethods);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted paymentMethods {PaymentMethodId}", id);

                return Ok(new ApiResponse<object>(null, "PaymentMethods deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting PaymentMethods {PaymentMethodsId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search PaymentMethod by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching PaymentMethod</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<PaymentMethodsdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<PaymentMethodsdto>>>> SearchPaymentMethods(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching paymentMethod for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var paymentMethod = await _unitOfWork.PaymentMethods.SearchPaymentMethodsAsync(tenantId, searchTerm);
                var paymentMethodDtos = _mapper.Map<List<PaymentMethodsdto>>(paymentMethod);

                _logger.LogInformation("Found {Count} paymentMethod matching search term", paymentMethodDtos.Count);

                return Ok(new ApiResponse<List<PaymentMethodsdto>>(paymentMethodDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching PaymentMethod");
                throw;
            }
        }

        /// <summary>
        /// Get active paymentMethod for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active payment Method</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<PaymentMethodsdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<PaymentMethodsdto>>>> GetActivePaymentMethod([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active PaymentMethod for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var paymentMethod = await _unitOfWork.PaymentMethods.GetActivePaymentMethodsAsync(tenantId);
                var paymentMethodDtos = _mapper.Map<List<PaymentMethodsdto>>(paymentMethod);

                _logger.LogInformation("Successfully retrieved {Count} active paymentMethods", paymentMethodDtos.Count);

                return Ok(new ApiResponse<List<PaymentMethodsdto>>(paymentMethodDtos, "Active Paymentmethod retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active PaymentMethod for tenant {TenantId}", tenantId);
                throw;
            }
        }



    }

}
