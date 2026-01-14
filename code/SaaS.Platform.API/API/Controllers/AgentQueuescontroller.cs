using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.AgentsQueues;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// AgentQueues management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class AgentQueuesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AgentQueuesController> _logger;
        private readonly IValidator<CreateAgentsQueuesdto> _createValidator;
        private readonly IValidator<UpdateAgentsQueuesdto> _updateValidator;

        public AgentQueuesController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AgentQueuesController> logger,
            IValidator<CreateAgentsQueuesdto> createValidator,
            IValidator<UpdateAgentsQueuesdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

       

        /// <summary>
        /// Get agentQueues by ID
        /// </summary>
        /// <param name="id">AgentQueues ID</param>
        /// <returns>AgentQueues details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AgentQueuesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<AgentQueuesdto>>> GetAgentQueuesById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching agentQueues with ID: {AgentQueuesId}", id);

                var agentQueues = await _unitOfWork.AgentQueues.GetByIdAsync(id);

                if (agentQueues == null)
                {
                    _logger.LogWarning("AgentQueues with ID {AgentQueuesId} not found", id);
                    return NotFound(new ApiResponse<object>($"AgentQueues with ID {id} not found"));
                }

                var agentQueuesDto = _mapper.Map<AgentQueuesdto>(agentQueues);

                _logger.LogInformation("Successfully retrieved agentQueues {AgentId}", id);

                return Ok(new ApiResponse<AgentQueuesdto>(agentQueuesDto, "AgentQueues retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching agentQueues {AgentId}", id);
                throw;
            }
        }

       
        /// Create a new agentQueues
        /// </summary>
        /// <param name="createAgentQueuesdto">Agent creation data</param>
        /// <returns>Created agentQueues details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AgentQueuesdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<AgentQueuesdto>>> CreateAgentQueues([FromBody] CreateAgentsQueuesdto createAgentQueuesdto)
        {
            try
            {
                
                

                // Map DTO to entity
                var agentQueues = _mapper.Map<AgentQueues>(createAgentQueuesdto);
                agentQueues.AgentQueuesId = Guid.NewGuid();
                agentQueues.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.AgentQueues.AddAsync(agentQueues);
                await _unitOfWork.SaveChangesAsync();

                var agentQueuesDto = _mapper.Map<AgentQueuesdto>(agentQueues);

                _logger.LogInformation("Successfully created agentQueues {AgentQueuesId} ",
                    agentQueues.AgentQueuesId);

                return CreatedAtAction(
                    nameof(GetAgentQueuesById),
                    new { id = agentQueues.AgentQueuesId },
                    new ApiResponse<AgentQueuesdto>(agentQueuesDto, "AgentQueues created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating agentQueues");
                throw;
            }
        }

        /// <summary>
        /// Update an existing agentQueues
        /// </summary>
        /// <param name="id">AgentQueues ID</param>
        /// <param name="updateAgentdto">AgentQueues update data</param>
        /// <returns>Updated agentQueues details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AgentQueuesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<AgentQueuesdto>>> UpdateAgentQueues(
            Guid id,
            [FromBody] UpdateAgentsQueuesdto updateAgentQueuesdto)
        {
            try
            {
                _logger.LogInformation("Updating agentQueues {AgentQueuesId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateAgentQueuesdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for AgentQueues update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing agentQueues
                var existingAgentQueues = await _unitOfWork.AgentQueues.GetByIdAsync(id);
                if (existingAgentQueues == null)
                {
                    _logger.LogWarning("AgentQueues {AgentQueuesId} not found", id);
                    return NotFound(new ApiResponse<object>($"AgentQueues with ID {id} not found"));
                }

                // Update only provided fields

                
                if (updateAgentQueuesdto.IsActive.HasValue)
                    existingAgentQueues.IsActive = updateAgentQueuesdto.IsActive.Value;


                _unitOfWork.AgentQueues.Update(existingAgentQueues);
                await _unitOfWork.SaveChangesAsync();

                var agentQueuesDto = _mapper.Map<AgentQueuesdto>(existingAgentQueues);

                _logger.LogInformation("Successfully updated agentQueues {AgentQueuesId}", id);

                return Ok(new ApiResponse<AgentQueuesdto>(agentQueuesDto, "AgentQueues updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating agentQueues {AgentQueuesId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a agentQueues (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">AgentQueues ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteAgentQueues(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting agentQueues {AgentQueuesId}", id);

                var agentQueues = await _unitOfWork.AgentQueues.GetByIdAsync(id);
                if (agentQueues == null)
                {
                    _logger.LogWarning("AgentQueues {AgentQueuesId} not found", id);
                    return NotFound(new ApiResponse<object>($"AgentQueues with ID {id} not found"));
                }

                // Soft delete
                agentQueues.IsActive = false;
                

                _unitOfWork.AgentQueues.Update(agentQueues);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted agentQueues {AgentQueuesId}", id);

                return Ok(new ApiResponse<object>(null, "AgentQueues deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting agentQueues {AgentQueuesId}", id);
                throw;
            }
        }

        
       
        }
    }
