using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Subscriptions;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// Subscription management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SubscriptionsController> _logger;
        private readonly IValidator<CreateSubscriptionsdto> _createValidator;
        private readonly IValidator<UpdateSubscriptionsdto> _updateValidator;

        public SubscriptionsController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<SubscriptionsController> logger,
            IValidator<CreateSubscriptionsdto> createValidator,
            IValidator<UpdateSubscriptionsdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all subscriptions with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="status"> status filter</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of subscriptions</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<Subscriptionsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<Subscriptionsdto>>> GetSubscriptions(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? status = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching subscriptions for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (subscription, totalCount) = await _unitOfWork.Subscriptions.GetSubscriptionsPagedAsync(
                    tenantId, searchTerm, status, pageNumber, pageSize);

                var subscriptionDtos = _mapper.Map<List<Subscriptionsdto>>(subscription);

                var response = new PagedResponse<Subscriptionsdto>(subscriptionDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} subscriptions out of {TotalCount} for tenant {TenantId}",
                    subscriptionDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching subscriptions for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get subscription by ID
        /// </summary>
        /// <param name="id">subscription ID</param>
        /// <returns>Subscription details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Subscriptionsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Subscriptionsdto>>> GetSubscriptionsById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching subscription with ID: {SubscriptionId}", id);

                var subscription = await _unitOfWork.Subscriptions.GetByIdAsync(id);

                if (subscription == null)
                {
                    _logger.LogWarning("Subscription with ID {SubscriptionId} not found", id);
                    return NotFound(new ApiResponse<object>($"subscription with ID {id} not found"));
                }

                var subscriptionDto = _mapper.Map<Subscriptionsdto>(subscription);

                _logger.LogInformation("Successfully retrieved subscription {SubscriptionId}", id);

                return Ok(new ApiResponse<Subscriptionsdto>(subscriptionDto, "Subscription retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching subscription {SubscriptionId}", id);
                throw;
            }
        }

        

        /// <summary>
        /// Create a new subscription
        /// </summary>
        /// <param name="createSubscriptionsdto">Subscription creation data</param>
        /// <returns>Created subscription details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Subscriptionsdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Subscriptionsdto>>> CreateSubscriptions([FromBody] CreateSubscriptionsdto createSubscriptionsdto)
        {
            try
            {
                _logger.LogInformation("Creating new subscription  for tenant {TenantId}",
                     createSubscriptionsdto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createSubscriptionsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for subscription creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

               

                // Map DTO to entity
                var subscription = _mapper.Map<Subscriptions>(createSubscriptionsdto);
                subscription.SubscriptionId = Guid.NewGuid();
                subscription.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.Subscriptions.AddAsync(subscription);
                await _unitOfWork.SaveChangesAsync();

                var subscriptionDto = _mapper.Map<Subscriptionsdto>(subscription);

                _logger.LogInformation("Successfully created subscription {SubscriptionId}",
                    subscription.SubscriptionId);

                return CreatedAtAction(
                    nameof(GetSubscriptionsById),
                    new { id = subscription.SubscriptionId },
                    new ApiResponse<Subscriptionsdto>(subscriptionDto, "Subscription created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating subscription");
                throw;
            }
        }

        /// <summary>
        /// Update an existing subscription
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <param name="updateSubscriptionsdto">Subscription update data</param>
        /// <returns>Updated customer details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Subscriptionsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Subscriptionsdto>>> UpdateSubscriptions(
            Guid id,
            [FromBody] UpdateSubscriptionsdto updateSubscriptionsdto)
        {
            try
            {
                _logger.LogInformation("Updating subscription {SubscriptionId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateSubscriptionsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for subscription update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing subscription
                var existingSubscription = await _unitOfWork.Subscriptions.GetByIdAsync(id);
                if (existingSubscription == null)
                {
                    _logger.LogWarning("Subscription {SubscriptionId} not found", id);
                    return NotFound(new ApiResponse<object>($"Subscription with ID {id} not found"));
                }

                // Update only provided fields
               
                if (!string.IsNullOrWhiteSpace(updateSubscriptionsdto.Status))
                    existingSubscription.Status = updateSubscriptionsdto.Status;

                existingSubscription.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Subscriptions.Update(existingSubscription);
                await _unitOfWork.SaveChangesAsync();

                var subscriptionDto = _mapper.Map<Subscriptionsdto>(existingSubscription);

                _logger.LogInformation("Successfully updated customer {CustomerId}", id);

                return Ok(new ApiResponse<Subscriptionsdto>(subscriptionDto, "Customer updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating subscription {SubscriptionId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a subscriptions (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Subscription ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteSubscriptions(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting subscription {SubscriptionId}", id);

                var subscription = await _unitOfWork.Subscriptions.GetByIdAsync(id);
                if (subscription == null)
                {
                    _logger.LogWarning("Subscription {SubscriptionId} not found", id);
                    return NotFound(new ApiResponse<object>($"Subscription with ID {id} not found"));
                }

                // Soft delete
                subscription.Status = "Inactive";
                subscription.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Subscriptions.Update(subscription);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted subscription {SubscriptionId}", id);

                return Ok(new ApiResponse<object>(null, "Subscription deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting subscription {SubscriptionId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search subscriptions by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching subscriptions</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<Subscriptionsdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Subscriptionsdto>>>> SearchSubscriptions(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching subscriptions for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var subscription = await _unitOfWork.Subscriptions.SearchSubscriptionsAsync(tenantId, searchTerm);
                var subscriptionDtos = _mapper.Map<List<Subscriptionsdto>>(subscription);

                _logger.LogInformation("Found {Count} subscriptions matching search term", subscriptionDtos.Count);

                return Ok(new ApiResponse<List<Subscriptionsdto>>(subscriptionDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching subscriptions");
                throw;
            }
        }

        /// <summary>
        /// Get active subscriptions for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active subscriptions</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<Subscriptionsdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Subscriptionsdto>>>> GetActiveSubscriptions([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active subscriptionss for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var subscription = await _unitOfWork.Subscriptions.GetActiveSubscriptionsAsync(tenantId);
                var subscriptionDtos = _mapper.Map<List<Subscriptionsdto>>(subscription);

                _logger.LogInformation("Successfully retrieved {Count} active subscriptions", subscriptionDtos.Count);

                return Ok(new ApiResponse<List<Subscriptionsdto>>(subscriptionDtos, "Active subscriptions retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active subscript for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}