using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Agents;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// Agent management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class AgentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AgentsController> _logger;
        private readonly IValidator<CreateAgentdto> _createValidator;
        private readonly IValidator<UpdateAgentdto> _updateValidator;

        public AgentsController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AgentsController> logger,
            IValidator<CreateAgentdto> createValidator,
            IValidator<UpdateAgentdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all agents with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="status">Agent status filter</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of customers</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<Agentdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<Agentdto>>> GetAgents(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? status = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching agents for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (agents, totalCount) = await _unitOfWork.Agents.GetAgentsPagedAsync(
                    tenantId, searchTerm, status,  pageNumber, pageSize);

                var agentDtos = _mapper.Map<List<Agentdto>>(agents);

                var response = new PagedResponse<Agentdto>(agentDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} agents out of {TotalCount} for tenant {TenantId}",
                    agentDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching agents for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get agents by ID
        /// </summary>
        /// <param name="id">Agent ID</param>
        /// <returns>Agent details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Agentdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Agentdto>>> GetAgentById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching agent with ID: {AgentId}", id);

                var agent = await _unitOfWork.Agents.GetByIdAsync(id);

                if (agent == null)
                {
                    _logger.LogWarning("Agent with ID {AgentId} not found", id);
                    return NotFound(new ApiResponse<object>($"Agent with ID {id} not found"));
                }

                var agentDto = _mapper.Map<Agentdto>(agent);

                _logger.LogInformation("Successfully retrieved agent {AgentId}", id);

                return Ok(new ApiResponse<Agentdto>(agentDto, "Agent retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching agent {AgentId}", id);
                throw;
            }
        }

        /// <summary>
        /// Get agent by agent code
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="agentCode">Agent code</param>
        /// <returns>Agent details</returns>
        [HttpGet("by-code/{agentCode}")]
        [ProducesResponseType(typeof(ApiResponse<Agentdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Agentdto>>> GetAgentByCode(
            [FromQuery] Guid tenantId,
            string agentCode)
        {
            try
            {
                _logger.LogInformation("Fetching agent with code {AgentCode} for tenant {TenantId}",
                    agentCode, tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var agent = await _unitOfWork.Agents.GetByAgentCodeAsync(tenantId, agentCode);

                if (agent == null)
                {
                    _logger.LogWarning("Agent with code {AgentCode} not found for tenant {TenantId}",
                        agentCode, tenantId);
                    return NotFound(new ApiResponse<object>($"Agent with code {agentCode} not found"));
                }

                var agentDto = _mapper.Map<Agentdto>(agent);

                _logger.LogInformation("Successfully retrieved agent with code {AgentCode}", agentCode);

                return Ok(new ApiResponse<Agentdto>(agentDto, "Agent retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching agent with code {AgentCode}", agentCode);
                throw;
            }
        }

        /// <summary>
        /// Create a new agent
        /// </summary>
        /// <param name="createAgentdto">Agent creation data</param>
        /// <returns>Created agent details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Agentdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Agentdto>>> CreateAgent([FromBody] CreateAgentdto createAgentdto)
        {
            try
            {
                _logger.LogInformation("Creating new agent with code {AgentCode} for tenant {TenantId}",
                    createAgentdto.AgentCode, createAgentdto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createAgentdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Check if agent code is unique
                var isUnique = await _unitOfWork.Agents.IsAgentCodeUniqueAsync(
                    createAgentdto.TenantId, createAgentdto.AgentCode);

                if (!isUnique)
                {
                    _logger.LogWarning("Agent code {AgentCode} already exists for tenant {TenantId}",
                        createAgentdto.AgentCode, createAgentdto.TenantId);
                    return BadRequest(new ApiResponse<object>("Agent code already exists"));
                }

                // Map DTO to entity
                var agent = _mapper.Map<Agents>(createAgentdto);
                agent.AgentId = Guid.NewGuid();
                agent.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.Agents.AddAsync(agent);
                await _unitOfWork.SaveChangesAsync();

                var agentDto = _mapper.Map<Agentdto>(agent);

                _logger.LogInformation("Successfully created agent {AgentId} with code {AgentCode}",
                    agent.AgentId, agent.AgentCode);

                return CreatedAtAction(
                    nameof(GetAgentById),
                    new { id = agent.AgentId },
                    new ApiResponse<Agentdto>(agentDto, "Agent created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating agent");
                throw;
            }
        }

        /// <summary>
        /// Update an existing agent
        /// </summary>
        /// <param name="id">Agent ID</param>
        /// <param name="updateAgentdto">Agent update data</param>
        /// <returns>Updated agent details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Agentdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Agentdto>>> UpdateAgent(
            Guid id,
            [FromBody] UpdateAgentdto updateAgentdto)
        {
            try
            {
                _logger.LogInformation("Updating agent {AgentId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateAgentdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing agent
                var existingAgent = await _unitOfWork.Agents.GetByIdAsync(id);
                if (existingAgent == null)
                {
                    _logger.LogWarning("Agent {AgentId} not found", id);
                    return NotFound(new ApiResponse<object>($"Agent with ID {id} not found"));
                }

                // Update only provided fields
  
               if (!string.IsNullOrWhiteSpace(updateAgentdto.AgentStatus))
                    existingAgent.AgentStatus = updateAgentdto.AgentStatus;

               if (updateAgentdto.IsActive.HasValue)
                    existingAgent.IsActive = updateAgentdto.IsActive.Value;

                existingAgent.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Agents.Update(existingAgent);
                await _unitOfWork.SaveChangesAsync();

                var agentDto = _mapper.Map<Agentdto>(existingAgent);

                _logger.LogInformation("Successfully updated agent {AgentId}", id);

                return Ok(new ApiResponse<Agentdto>(agentDto, "Agent updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating agent {AgentId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a agent (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Agent ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteAgent(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting agent {AgentId}", id);

                var agent = await _unitOfWork.Agents.GetByIdAsync(id);
                if (agent == null)
                {
                    _logger.LogWarning("Agent {AgentId} not found", id);
                    return NotFound(new ApiResponse<object>($"Agent with ID {id} not found"));
                }

                // Soft delete
                agent.IsActive = false;
                agent.AgentStatus = "Inactive";
                agent.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Agents.Update(agent);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted agent {AgentId}", id);

                return Ok(new ApiResponse<object>(null, "Agent deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting agent {AgentId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search agent by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching agents</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<Agentdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Agentdto>>>> SearchAgents(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching agents for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var agents = await _unitOfWork.Agents.SearchAgentsAsync(tenantId, searchTerm);
                var agentDtos = _mapper.Map<List<Agentdto>>(agents);

                _logger.LogInformation("Found {Count} agent matching search term", agentDtos.Count);

                return Ok(new ApiResponse<List<Agentdto>>(agentDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching agents");
                throw;
            }
        }

        /// <summary>
        /// Get active agents for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active agents</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<Agentdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Agentdto>>>> GetActiveAgent([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active agent for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var agent = await _unitOfWork.Agents.GetActiveAgentsAsync(tenantId);
                var agentDtos = _mapper.Map<List<Agentdto>>(agent);

                _logger.LogInformation("Successfully retrieved {Count} active agents", agentDtos.Count);

                return Ok(new ApiResponse<List<Agentdto>>(agentDtos, "Active agent retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active agents for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}