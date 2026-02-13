using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Customer;
using SaaS.Platform.API.Application.DTOs.WorkflowTask;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// WorkflowTasks management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class WorkflowTasksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<WorkflowTasksController> _logger;
        private readonly IValidator<CreateWorkflowTaskdto> _createValidator;
        private readonly IValidator<UpdateWorkflowTaskdto> _updateValidator;

        public WorkflowTasksController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<WorkflowTasksController> logger,
            IValidator<CreateWorkflowTaskdto> createValidator,
            IValidator<UpdateWorkflowTaskdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }
        /// <summary>
        /// Get workflowTask by ID
        /// </summary>
        /// <param name="id">WorkflowTask ID</param>
        /// <returns>WorkflowTask details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<WorkflowTaskdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<WorkflowTaskdto>>> GetWorkflowTaskById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching workflowTask with ID: {WorkflowTaskId}", id);

                var workflowTask = await _unitOfWork.WorkflowTask.GetByIdAsync(id);

                if (workflowTask == null)
                {
                    _logger.LogWarning("WorkflowTask with ID {WorkFlowTaskId} not found", id);
                    return NotFound(new ApiResponse<object>($"WorkflowTask with ID {id} not found"));
                }

                var workflowTaskDto = _mapper.Map<WorkflowTaskdto>(workflowTask);

                _logger.LogInformation("Successfully retrieved workflowTask {WorkflowTaskId}", id);

                return Ok(new ApiResponse<WorkflowTaskdto>(workflowTaskDto, "WorkflowTask retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching workflowTask  {WorkflowTaskId}", id);
                throw;
            }
        }
    }
}