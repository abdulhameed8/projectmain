using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs;
using SaaS.Platform.API.Application.DTOs.Payment;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// Payment management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class PaymentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentsController> _logger;
        private readonly IValidator<CreatePaymentsdto> _createValidator;
        private readonly IValidator<UpdatePaymentsdto> _updateValidator;

        public PaymentsController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<PaymentsController> logger,
            IValidator<CreatePaymentsdto> createValidator,
            IValidator<UpdatePaymentsdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all Payments with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of Payments</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<Paymentsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<Paymentsdto>>> GetPayments(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? status = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching Payments for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (payment, totalCount) = await _unitOfWork.Payments.GetPaymentsPagedAsync(
                    tenantId, searchTerm, status, pageNumber, pageSize);

                var paymentDtos = _mapper.Map<List<Paymentsdto>>(payment);

                var response = new PagedResponse<Paymentsdto>(paymentDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} Payments out of {TotalCount} for tenant {TenantId}",
                    paymentDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching Payments for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// </summary>
        /// <param name="id">Payments ID</param>
        /// <returns>Payments details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Paymentsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]

        public async Task<ActionResult<ApiResponse<Paymentsdto>>> GetPaymentsById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching Payments with ID: {PaymentId}", id);

                var payment = await _unitOfWork.Payments.GetByIdAsync(id);

                if (payment == null)
                {
                    _logger.LogWarning("Payments with ID {PaymentId} not found", id);
                    return NotFound(new ApiResponse<object>($"Payment with ID {id} not found"));
                }

                var paymentDto = _mapper.Map<Paymentsdto>(payment);

                _logger.LogInformation("Successfully retrieved Payments {PaymentId}", id);

                return Ok(new ApiResponse<Paymentsdto>(paymentDto, "Payments retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching Payments {PaymentId}", id);
                throw;
            }
        }

        /// <summary>
        /// Create a new Payment
        /// </summary>
        /// <param name="createPaymentsdto">Payment creation data</param>
        /// <returns>Created Payments details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Paymentsdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Paymentsdto>>> CreatePayments([FromBody] CreatePaymentsdto createPaymentsdto)
        {
            try
            {
                _logger.LogInformation("Creating new Payments  for tenant {TenantId}",
                     createPaymentsdto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createPaymentsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for Payment creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }


                // Map DTO to entity
                var payment = _mapper.Map<Payments>(createPaymentsdto);
                payment.PaymentMethodId = Guid.NewGuid();
                payment.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.Payments.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                var paymentDto = _mapper.Map<Paymentsdto>(payment);

                _logger.LogInformation("Successfully created Payment {PaymentId} )",
                    payment.PaymentId);

                return CreatedAtAction(
                    nameof(GetPaymentsById),
                    new { id = payment.PaymentId },
                    new ApiResponse<Paymentsdto>(paymentDto, "Payments created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating payment");
                throw;
            }
        }

        /// <summary>
        /// Update an existing Payment
        /// </summary>
        /// <param name="id">Payment ID</param>
        /// <param name="updatePaymentsDto">Payment update data</param>
        /// <returns>Updated Payments details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Paymentsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Paymentsdto>>> UpdatePayments(
            Guid id,
            [FromBody] UpdatePaymentsdto updatePaymentsdto)
        {
            try
            {
                _logger.LogInformation("Updating Payment  {PaymentId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updatePaymentsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for Payment update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing customer
                var existingPayment = await _unitOfWork.Payments.GetByIdAsync(id);
                if (existingPayment == null)
                {
                    _logger.LogWarning("Payment {PaymentId} not found", id);
                    return NotFound(new ApiResponse<object>($"Payment with ID {id} not found"));
                }

                // Update only provided fields


                _unitOfWork.Payments.Update(existingPayment);
                await _unitOfWork.SaveChangesAsync();

                var paymentDto = _mapper.Map<Paymentsdto>(existingPayment);

                _logger.LogInformation("Successfully updated Payments {PaymentId}", id);

                return Ok(new ApiResponse<Paymentsdto>(paymentDto, "Payments updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating Payments {PaymentId}", id);
                throw;
            }
        }
        /// <summary>
        /// Delete a Payments (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Payments ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeletePayments(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting Payments {PaymentId}", id);

                var payment = await _unitOfWork.Payments.GetByIdAsync(id);
                if (payment == null)
                {
                    _logger.LogWarning("Payment {PaymentId} Payment not found", id);
                    return NotFound(new ApiResponse<object>($"Payments with ID {id} not found"));
                }

                // Soft delete

                _unitOfWork.Payments.Update(payment);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted payment {PaymentId}", id);

                return Ok(new ApiResponse<object>(null, "Payment deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting Payments {PaymentId}", id);
                throw;
            }
        }


    }
}