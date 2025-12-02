using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Customer;
using SaaS.Platform.API.Application.DTOs.SubscriptionsPlan;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// SubscriptionPlan management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class SubscriptionsPlanController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SubscriptionsPlanController> _logger;
        private readonly IValidator<CreateSubscriptionsPlandto> _createValidator;
        private readonly IValidator<UpdateSubscriptionsPlandto> _updateValidator;

        public SubscriptionsPlanController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<SubscriptionsPlanController> logger,
            IValidator<CreateSubscriptionsPlandto> createValidator,
            IValidator<UpdateSubscriptionsPlandto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }




        /// <summary>
        /// Get subscriptionsPlan by ID
        /// </summary>
        /// <param name="id">SubscriptionsPlan ID</param>
        /// <returns>SubscriptionsPlan details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SubscriptionsPlanDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<SubscriptionsPlanDto>>> GetSubscriptionsPlanById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching subscriptionsPlan with ID: {SubscriptionsPlanId}", id);

                var subscriptionsPlan = await _unitOfWork.SubscriptionsPlans.GetByIdAsync(id);

                if (subscriptionsPlan == null)
                {
                    _logger.LogWarning("SubscriptionsPlan with ID {SubscriptionsPlanId} not found", id);
                    return NotFound(new ApiResponse<object>($"SubscriptionsPlan with ID {id} not found"));
                }

                var subscriptionsPlanDto = _mapper.Map<SubscriptionsPlanDto>(subscriptionsPlan);

                _logger.LogInformation("Successfully retrieved subscriptionsPlan {SubscriptionsPlanId}", id);

                return Ok(new ApiResponse<SubscriptionsPlanDto>(subscriptionsPlanDto, "SubscriptionsPlan retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching subscriptionsPlan {SubscriptionsPlanId}", id);
                throw;
            }
        }


        /// <summary>
        /// Update an existing subscriptionsPlan
        /// </summary>
        /// <param name="id">SubscriptionsPlan ID</param>
        /// <param name="updateSubscriptionsPlanDto">SubscriptionsPlan update data</param>
        /// <returns>Updated subscriptionsPlan details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SubscriptionsPlanDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<SubscriptionsPlanDto>>> UpdateSubscriptionsPlan(
            Guid id,
            [FromBody] UpdateSubscriptionsPlandto updateSubscriptionsPlanDto)
        {
            try
            {
                _logger.LogInformation("Updating subscriptionsPlan {SubscriptionsPlanId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateSubscriptionsPlanDto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for subscripionsPlan update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing subscriptionsPlan
                var existingSubscriptionsPlan = await _unitOfWork.SubscriptionsPlans.GetByIdAsync(id);
                if (existingSubscriptionsPlan == null)
                {
                    _logger.LogWarning("SubscriptionsPlan {SubscriptionsPlanId} not found", id);
                    return NotFound(new ApiResponse<object>($"SubscriptionsPlan with ID {id} not found"));
                }

                // Update only provided fields
               

                    if (updateSubscriptionsPlanDto.IsActive.HasValue)
                    existingSubscriptionsPlan.IsActive = updateSubscriptionsPlanDto.IsActive.Value;

                existingSubscriptionsPlan.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.SubscriptionsPlans.Update(existingSubscriptionsPlan);
                await _unitOfWork.SaveChangesAsync();

                var subscriptionsPlanDto = _mapper.Map<SubscriptionsPlanDto>(existingSubscriptionsPlan);

                _logger.LogInformation("Successfully updated subscriptionsPlan {SubscriptionsPlanId}", id);

                return Ok(new ApiResponse<SubscriptionsPlanDto>(subscriptionsPlanDto, "SubscriptionsPlan updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating customer {CustomerId}", id);
                throw;
            }
        }

    }
}