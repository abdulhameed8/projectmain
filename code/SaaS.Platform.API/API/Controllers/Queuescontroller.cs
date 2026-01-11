using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Queues;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// Queues management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class QueuesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<QueuesController> _logger;
        private readonly IValidator<CreateQueuedto> _createValidator;
        private readonly IValidator<UpdateQueuedto> _updateValidator;

        public QueuesController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<QueuesController> logger,
            IValidator<CreateQueuedto> createValidator,
            IValidator<UpdateQueuedto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all queues with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of queues</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<Queuedto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<Queuedto>>> GetQueues(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching queues for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (queues, totalCount) = await _unitOfWork.Queues.GetQueuesPagedAsync(
                    tenantId, searchTerm,  pageNumber, pageSize);

                var queueDtos = _mapper.Map<List<Queuedto>>(queues);

                var response = new PagedResponse<Queuedto>(queueDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} queues out of {TotalCount} for tenant {TenantId}",
                    queueDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching queues for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get queues by ID
        /// </summary>
        /// <param name="id">Queues ID</param>
        /// <returns>Queues details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Queuedto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Queuedto>>> GetQueueById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching queue with ID: {QueueId}", id);

                var queue = await _unitOfWork.Queues.GetByIdAsync(id);

                if (queue == null)
                {
                    _logger.LogWarning("Queue with ID {QueueId} not found", id);
                    return NotFound(new ApiResponse<object>($"Queue with ID {id} not found"));
                }

                var queueDto = _mapper.Map<Queuedto>(queue);

                _logger.LogInformation("Successfully retrieved queue {QueueId}", id);

                return Ok(new ApiResponse<Queuedto>(queueDto, "Queue retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching queue {QueueId}", id);
                throw;
            }
        }

        /// <summary>
        /// Get queue by queue code
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="customerCode">Queue code</param>
        /// <returns>Queue details</returns>
        [HttpGet("by-code/{queueCode}")]
        [ProducesResponseType(typeof(ApiResponse<Queuedto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Queuedto>>> GetQueueByCode(
            [FromQuery] Guid tenantId,
            string queueCode)
        {
            try
            {
                _logger.LogInformation("Fetching queue with code {QueueCode} for tenant {TenantId}",
                    queueCode, tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var queue = await _unitOfWork.Queues.GetByQueueCodeAsync(tenantId, queueCode);

                if (queue == null)
                {
                    _logger.LogWarning("Queue with code {QueueCode} not found for tenant {TenantId}",
                        queueCode, tenantId);
                    return NotFound(new ApiResponse<object>($"Queue with code {queueCode} not found"));
                }

                var queuedto = _mapper.Map<Queuedto>(queue);

                _logger.LogInformation("Successfully retrieved queue with code {QueueCode}", queueCode);

                return Ok(new ApiResponse<Queuedto>(queuedto, "Queue retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching queue with code {QueueCode}", queueCode);
                throw;
            }
        }

        /// <summary>
        /// Create a new queue
        /// </summary>
        /// <param name="createQueuedto">Queue creation data</param>
        /// <returns>Created Queue details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Queuedto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Queuedto>>> CreateQueue([FromBody] CreateQueuedto createQueuedto)
        {
            try
            {
                _logger.LogInformation("Creating new queue with code {QueueCode} for tenant {TenantId}",
                    createQueuedto.QueueCode, createQueuedto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createQueuedto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for queue creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Check if queue code is unique
                var isUnique = await _unitOfWork.Queues.IsQueuesCodeUniqueAsync(
                    createQueuedto.TenantId, createQueuedto.QueueCode);

                if (!isUnique)
                {
                    _logger.LogWarning("Queue code {QueueCode} already exists for tenant {TenantId}",
                        createQueuedto.QueueCode, createQueuedto.TenantId);
                    return BadRequest(new ApiResponse<object>("Queue code already exists"));
                }

                // Map DTO to entity
                var queue = _mapper.Map<Queues>(createQueuedto);
                queue.QueueId = Guid.NewGuid();
                queue.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.Queues.AddAsync(queue);
                await _unitOfWork.SaveChangesAsync();

                var queuedto = _mapper.Map<Queuedto>(queue);

                _logger.LogInformation("Successfully created queue {QueueId} with code {QueueCode}",
                    queue.QueueId, queue.QueueCode);

                return CreatedAtAction(
                    nameof(GetQueueById),
                    new { id = queue.QueueId },
                    new ApiResponse<Queuedto>(queuedto, "Queue created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating queue");
                throw;
            }
        }

        /// <summary>
        /// Update an existing queue
        /// </summary>
        /// <param name="id">Queue ID</param>
        /// <param name="updateQueuedto">Queue update data</param>
        /// <returns>Updated queue details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Queuedto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Queuedto>>> UpdateQueue(
            Guid id,
            [FromBody] UpdateQueuedto updateQueuedto)
        {
            try
            {
                _logger.LogInformation("Updating queue {QueueId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateQueuedto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for queue update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing queue
                var existingQueue = await _unitOfWork.Queues.GetByIdAsync(id);
                if (existingQueue == null)
                {
                    _logger.LogWarning("Queue {QueueId} not found", id);
                    return NotFound(new ApiResponse<object>($"Queue with ID {id} not found"));
                }

                // Update only provided fields
                
                if (!string.IsNullOrWhiteSpace(updateQueuedto.QueueName))
                    existingQueue.QueueName = updateQueuedto.QueueName;

                if (updateQueuedto.MaxQueueSize.HasValue)
                    existingQueue.MaxQueueSize = updateQueuedto.MaxQueueSize.Value;

                if (updateQueuedto.IsActive.HasValue)
                    existingQueue.IsActive = updateQueuedto.IsActive.Value;

                existingQueue.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Queues.Update(existingQueue);
                await _unitOfWork.SaveChangesAsync();

                var queueDto = _mapper.Map<Queuedto>(existingQueue);

                _logger.LogInformation("Successfully updated queue {QueueId}", id);

                return Ok(new ApiResponse<Queuedto>(queueDto, "Queue updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating queue {QueueId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a queue (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Queue ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteQueue(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting queue {QueueId}", id);

                var queue = await _unitOfWork.Queues.GetByIdAsync(id);
                if (queue == null)
                {
                    _logger.LogWarning("Queue {QueueId} not found", id);
                    return NotFound(new ApiResponse<object>($"Queue with ID {id} not found"));
                }

                // Soft delete
                queue.IsActive = false;
                queue.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Queues.Update(queue);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted queue {QueueId}", id);

                return Ok(new ApiResponse<object>(null, "Queue deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting queue {QueueId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search queues by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching queues</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<Queuedto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Queuedto>>>> SearchQueues(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching queues for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var queues = await _unitOfWork.Queues.SearchQueuesAsync(tenantId, searchTerm);
                var queueDtos = _mapper.Map<List<Queuedto>>(queues);

                _logger.LogInformation("Found {Count} queues matching search term", queueDtos.Count);

                return Ok(new ApiResponse<List<Queuedto>>(queueDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching queue");
                throw;
            }
        }

        /// <summary>
        /// Get active queues for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active queues</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<Queuedto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Queuedto>>>> GetActiveQueues([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active queues for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var queues = await _unitOfWork.Queues.GetActiveQueuesAsync(tenantId);
                var queueDtos = _mapper.Map<List<Queuedto>>(queues);

                _logger.LogInformation("Successfully retrieved {Count} active queues", queueDtos.Count);

                return Ok(new ApiResponse<List<Queuedto>>(queueDtos, "Active queues retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active queues for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}