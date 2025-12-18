using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Activities;
using SaaS.Platform.API.Application.DTOs.AuditLogs;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;
using System.Diagnostics;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// Activity management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class ActivityController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ActivityController> _logger;
        private readonly IValidator<CreateActivitydto> _createValidator;
        private readonly IValidator<UpdateActivitydto> _updateValidator;

        public ActivityController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ActivityController> logger,
            IValidator<CreateActivitydto> createValidator,
            IValidator<UpdateActivitydto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all activities with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="status">Activity status filter</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of customers</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<Activitydto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<Activitydto>>> GetActivity(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? status = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching activity for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (activities, totalCount) = await _unitOfWork.Activity.GetActivityPagedAsync(
                    tenantId, searchTerm, status, pageNumber, pageSize);

                var activityDtos = _mapper.Map<List<Activitydto>>(activities);

                var response = new PagedResponse<Activitydto>(activityDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} activity out of {TotalCount} for tenant {TenantId}",
                    activityDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching activity for tenant {TenantId}", tenantId);
                throw;
            }
        }


        /// <summary>
        /// Get activity by ID
        /// </summary>
        /// <param name="id">Activity ID</param>
        /// <returns>Activity details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Activitydto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Activitydto>>> GetActivityById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching activity with ID: {ActivityId}", id);

                var activity = await _unitOfWork.Activity.GetByIdAsync(id);

                if (activity == null)
                {
                    _logger.LogWarning("Activity with ID {ActivityId} not found", id);
                    return NotFound(new ApiResponse<object>($"Activity with ID {id} not found"));
                }

                var activityDto = _mapper.Map<Activitydto>(activity);

                _logger.LogInformation("Successfully retrieved activity {ActivityId}", id);

                return Ok(new ApiResponse<Activitydto>(activityDto, "Activity retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching activity {ActivityId}", id);
                throw;
            }
        }


        /// <summary>
        /// Create a new activity
        /// </summary>
        /// <param name="createActivitydto">Activity creation data</param>
        /// <returns>Created activity details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Activitydto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Activitydto>>> CreateActivity([FromBody] CreateActivitydto createActivitydto)
        {
            try
            {
                _logger.LogInformation("Creating new activity  for tenant {TenantId}",
                      createActivitydto.TenantId);


                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createActivitydto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for activity creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }



                // Map DTO to entity
                var activity = _mapper.Map<Activities>(createActivitydto);
                activity.ActivityId = Guid.NewGuid();
                activity.CreatedDate = DateTime.UtcNow;


                // Add to database
                 await _unitOfWork.Activity.AddAsync(activity);
                await _unitOfWork.SaveChangesAsync();

                var activityDto = _mapper.Map<Activitydto>(activity);

                _logger.LogInformation("Successfully created activity {ActivityId} ",
                    activity.ActivityId);

                return CreatedAtAction(
                    nameof(GetActivityById),
                    new { id = activity.ActivityId },
                    new ApiResponse<Activitydto>(activityDto, "Activity created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating activity");
                throw;
            }
        }

        /// <summary>
        /// Update an existing activity
        /// </summary>
        /// <param name="id">Activity ID</param>
        /// <param name="updateActivitydto">Activity update data</param>
        /// <returns>Updated activity details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Activitydto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Activitydto>>> UpdateActivity(
            Guid id,
            [FromBody] UpdateActivitydto updateActivitydto)
        {
            try
            {
                _logger.LogInformation("Updating activity {ActivityId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateActivitydto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for activity update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing activity
                var existingActivity = await _unitOfWork.Activity.GetByIdAsync(id);
                if (existingActivity == null)
                {
                    _logger.LogWarning("Activity {ActivityId} not found", id);
                    return NotFound(new ApiResponse<object>($"Activity with ID {id} not found"));
                }

                // Update only provided fields

                if (!string.IsNullOrWhiteSpace(updateActivitydto.ActivityStatus))
                    existingActivity.ActivityStatus = updateActivitydto.ActivityStatus;

                if (updateActivitydto.AssignedUserId.HasValue)
                    existingActivity.AssignedUserId = updateActivitydto.AssignedUserId;

                existingActivity.ModifiedDate = DateTime.UtcNow;



                _unitOfWork.Activity.Update(existingActivity);
                await _unitOfWork.SaveChangesAsync();

                var activityDto = _mapper.Map<Activitydto>(existingActivity);

                _logger.LogInformation("Successfully updated activity {ActivityId}", id);

                return Ok(new ApiResponse<Activitydto>(activityDto, "Activity updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating activity {ActivityId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a activity (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Activity ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteActivity(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting activity {ActivityId}", id);

                var activity = await _unitOfWork.Activity.GetByIdAsync(id);
                if (activity == null)
                {
                    _logger.LogWarning("Activity {ActivityId} not found", id);
                    return NotFound(new ApiResponse<object>($"Activity with ID {id} not found"));
                }

                // Soft delete
                activity.ActivityStatus = "Inactive";
                activity.ModifiedDate = DateTime.UtcNow;



                _unitOfWork.Activity.Update(activity);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted activity {ActivityId}", id);

                return Ok(new ApiResponse<object>(null, "Activity deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting activity {ActivityId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search activity by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching acitivity</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<Activitydto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Activitydto>>>> SearchActivity(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching acitivity for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var activity = await _unitOfWork.Activity.SearchActivityAsync(tenantId, searchTerm);
                var activityDtos = _mapper.Map<List<Activitydto>>(activity);

                _logger.LogInformation("Found {Count} activity matching search term", activityDtos.Count);

                return Ok(new ApiResponse<List<Activitydto>>(activityDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching customers");
                throw;
            }
        }

        /// <summary>
        /// Get active activity for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active activity</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<Activitydto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Activitydto>>>> GetActiveActivity([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active activity for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var activity = await _unitOfWork.Activity.GetActiveActivityAsync(tenantId);
                var activityDtos = _mapper.Map<List<Activitydto>>(activity);

                _logger.LogInformation("Successfully retrieved {Count} active acitvity", activityDtos.Count);

                return Ok(new ApiResponse<List<Activitydto>>(activityDtos, "Active activity retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active activity for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}

        
   