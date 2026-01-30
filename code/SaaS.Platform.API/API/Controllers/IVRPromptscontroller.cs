using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.IVRPrompts;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// IVRPrompt management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class IVRPromptsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<IVRPromptsController> _logger;
        private readonly IValidator<CreateIVRPromptsdto> _createValidator;
        private readonly IValidator<UpdateIVRPromptsdto> _updateValidator;

        public IVRPromptsController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<IVRPromptsController> logger,
            IValidator<CreateIVRPromptsdto> createValidator,
            IValidator<UpdateIVRPromptsdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all IVRPrompts with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of IVRPrompts</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<IVRPromptsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<IVRPromptsdto>>> GetIVRPrompts(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching IVRPrompts for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (iVRPrompts, totalCount) = await _unitOfWork.IVRPrompts.GetIVRPromptsPagedAsync(
                    tenantId, searchTerm, pageNumber, pageSize);

                var iVRPromptsDtos = _mapper.Map<List<IVRPromptsdto>>(iVRPrompts);

                var response = new PagedResponse<IVRPromptsdto>(iVRPromptsDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} iVRPrompts out of {TotalCount} for tenant {TenantId}",
                    iVRPromptsDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching iVRPrompts for tenant {TenantId}", tenantId);
                throw;
            }
        }
        /// <summary>
        /// Get Prompt by ID
        /// </summary>
        /// <param name="id">Prompt ID</param>
        /// <returns>IVRPrompt details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<IVRPromptsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<IVRPromptsdto>>> GetPromptsById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching iVRPrompt with ID: {PromptId}", id);

                var iVRPrompt = await _unitOfWork.IVRPrompts.GetByIdAsync(id);

                if (iVRPrompt == null)
                {
                    _logger.LogWarning("IVRPrompt with ID {PromptId} not found", id);
                    return NotFound(new ApiResponse<object>($"Prompt with ID {id} not found"));
                }

                var iVRPromptsDto = _mapper.Map<IVRPromptsdto>(iVRPrompt);

                _logger.LogInformation("Successfully retrieved iVRPrompt {PromptId}", id);

                return Ok(new ApiResponse<IVRPromptsdto>(iVRPromptsDto, "IVRPrompt retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching iVRPrompt {PromptId}", id);
                throw;
            }
        }


        /// <summary>
        /// Create a new iVRPrompt
        /// </summary>
        /// <param name="createIVRPromptsdto">IVRPrompts creation data</param>
        /// <returns>Created iVRPrompt details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<IVRPromptsdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<IVRPromptsdto>>> CreateIVRPrompts([FromBody] CreateIVRPromptsdto createIVRPromptsdto)
        {
            try
            {

                _logger.LogInformation("Creating new iVRPrompt  for tenant {TenantId}",
                         createIVRPromptsdto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createIVRPromptsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                

                // Map DTO to entity
                var iVRPrompt = _mapper.Map<IVRPrompts>(createIVRPromptsdto);
                iVRPrompt.PromptId = Guid.NewGuid();
                iVRPrompt.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.IVRPrompts.AddAsync(iVRPrompt);
                await _unitOfWork.SaveChangesAsync();

                var iVRPromptsDto = _mapper.Map<IVRPromptsdto>(iVRPrompt);

                _logger.LogInformation("Successfully created iVRPrompt {PromptId}",
                    iVRPrompt.PromptId);

                return CreatedAtAction(
                    nameof(GetPromptsById),
                    new { id = iVRPrompt.PromptId },
                    new ApiResponse<IVRPromptsdto>(iVRPromptsDto, "IVRPrompts created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating IVRPrompt");
                throw;
            }
        }

        /// <summary>
        /// Update an existing iVRPrompt
        /// </summary>
        /// <param name="id">IVRPrompts ID</param>
        /// <param name="updateIVRPromptsdto">IVRPrompt update data</param>
        /// <returns>Updated iVRPrompts details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<IVRPromptsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<IVRPromptsdto>>> UpdateIVRprompts(
            Guid id,
            [FromBody] UpdateIVRPromptsdto updateIVRPromptsdto)
        {
            try
            {
                _logger.LogInformation("Updating iVRPrompt {PromptId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateIVRPromptsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing customer
                var existingIVRPrompts = await _unitOfWork.IVRPrompts.GetByIdAsync(id);
                if (existingIVRPrompts == null)
                {
                    _logger.LogWarning("IVRPrompt {PromptId} not found", id);
                    return NotFound(new ApiResponse<object>($"IVRPrompt with ID {id} not found"));
                }

                // Update only provided fields

                


               

                _unitOfWork.IVRPrompts.Update(existingIVRPrompts);
                await _unitOfWork.SaveChangesAsync();

                var iVRPromptDto = _mapper.Map<IVRPromptsdto>(existingIVRPrompts);

                _logger.LogInformation("Successfully updated iVRPrompt {PromptId}", id);

                return Ok(new ApiResponse<IVRPromptsdto>(iVRPromptDto, "IVRPrompt updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating IVRPrompt {PromptId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a iVRPrompts (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Prompt ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeletePrompts(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting iVRPrompt {PromptId}", id);

                var iVRPrompt = await _unitOfWork.IVRPrompts.GetByIdAsync(id);
                if (iVRPrompt == null)
                {
                    _logger.LogWarning("IVRPrompt {PromptId} not found", id);
                    return NotFound(new ApiResponse<object>($"Prompt with ID {id} not found"));
                }

                // Soft delete
              
                _unitOfWork.IVRPrompts.Update(iVRPrompt);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted iVRPrompt {PromptId}", id);

                return Ok(new ApiResponse<object>(null, "Prompt deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting prompt {PromptId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search iVRPrompt by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching iVRPrompt</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<IVRPromptsdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<IVRPromptsdto>>>> SearchIVRPrompts(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching iVRPrompt for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var iVRPrompts = await _unitOfWork.IVRPrompts.SearchIVRPromptsAsync(tenantId, searchTerm);
                var iVRPromptDtos = _mapper.Map<List<IVRPromptsdto>>(iVRPrompts);

                _logger.LogInformation("Found {Count} iVRPrompt matching search term", iVRPromptDtos.Count);

                return Ok(new ApiResponse<List<IVRPromptsdto>>(iVRPromptDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching iVRPrompt");
                throw;
            }
        }

        /// <summary>
        /// Get active iVRPrompts for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active iVRPrompt</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<IVRPromptsdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<IVRPromptsdto>>>> GetActiveIVRPrompts([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active iVRPrompt for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var iVRPrompts = await _unitOfWork.IVRPrompts.GetActiveIVRPromptsAsync(tenantId);
                var iVRPromptDtos = _mapper.Map<List<IVRPromptsdto>>(iVRPrompts);

                _logger.LogInformation("Successfully retrieved {Count} active iVRPrompt", iVRPromptDtos.Count);

                return Ok(new ApiResponse<List<IVRPromptsdto>>(iVRPromptDtos, "Active iVRPrompts retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active iVRPrompts for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}