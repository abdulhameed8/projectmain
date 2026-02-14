using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.WorkflowHistory;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// WorkflowHistory management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class WorkflowHistoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<WorkflowHistoryController> _logger;
        private readonly IValidator<CreateWorkflowHistorydto> _createValidator;
        private readonly IValidator<UpdateWorkflowHistorydto> _updateValidator;

        public WorkflowHistoryController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<WorkflowHistoryController> logger,
            IValidator<CreateWorkflowHistorydto> createValidator,
            IValidator<UpdateWorkflowHistorydto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }
        /// <summary>
        /// Get workflowHistory by ID
        /// </summary>
        /// <param name="id">WorkflowHistory ID</param>
        /// <returns>WorkflowHistory details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<WorkflowHistorydto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<WorkflowHistorydto>>> GetHistoryById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching workflowHistory with ID: {WorkflowHistoryId}", id);

                var workflowHistory = await _unitOfWork.WorkflowHistory.GetByIdAsync(id);

                 if (workflowHistory == null)
                {
                    _logger.LogWarning("WorkflowHistory with ID {WorkFlowHistoryId} not found", id);
                    return NotFound(new ApiResponse<object>($"WorkflowHistory with ID {id} not found"));
                }

                var workflowHistoryDto = _mapper.Map<WorkflowHistorydto>(workflowHistory);

                _logger.LogInformation("Successfully retrieved workflowHistory {WorkflowHistoryId}", id);

                return Ok(new ApiResponse<WorkflowHistorydto>(workflowHistoryDto, "WorkflowHistory retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching workflowHistory  {WorkflowHistoryId}", id);
                throw;
            }
        }
    }
}