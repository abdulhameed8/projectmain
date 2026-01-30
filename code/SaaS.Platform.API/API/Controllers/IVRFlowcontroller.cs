using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.IVRFlows;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// IVRFlows management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class IVRFlowController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<IVRFlowController> _logger;
        private readonly IValidator<CreateIVRFlowsdto> _createValidator;
        private readonly IValidator<UpdateIVRFlowsdto> _updateValidator;

        public IVRFlowController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<IVRFlowController> logger,
            IValidator<CreateIVRFlowsdto> createValidator,
            IValidator<UpdateIVRFlowsdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all IVRFlows with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of IVrflows</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<IVRFlowsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<IVRFlowsdto>>> GetIVRFlows(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
             [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching iVRFlows for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (iVRFlows, totalCount) = await _unitOfWork.IVRFlows.GetIVRFlowsPagedAsync(
                    tenantId, searchTerm, pageNumber, pageSize);

                var iVRFlowsDtos = _mapper.Map<List<IVRFlowsdto>>(iVRFlows);

                var response = new PagedResponse<IVRFlowsdto>(iVRFlowsDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} iVRFlows out of {TotalCount} for tenant {TenantId}",
                    iVRFlowsDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching iVRFlows for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get IVRFlows by ID
        /// </summary>
        /// <param name="id">IVRFlow ID</param>
        /// <returns>IVRFlow details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<IVRFlowsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<IVRFlowsdto>>> GetIVRFlowsById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching iVRFlows with ID: {IVRFlowId}", id);

                var iVRFlows = await _unitOfWork.IVRFlows.GetByIdAsync(id);

                if (iVRFlows == null)
                {
                    _logger.LogWarning("IVRFlow with ID {IVRFlowId} not found", id);
                    return NotFound(new ApiResponse<object>($"IVRflow with ID {id} not found"));
                }

                var iVRFlowsDto = _mapper.Map<IVRFlowsdto>(iVRFlows);

                _logger.LogInformation("Successfully retrieved iVRFlows {IVRFlowId}", id);

                return Ok(new ApiResponse<IVRFlowsdto>(iVRFlowsDto, "IVRFlows retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching iVRFLows {IVRFlowId}", id);
                throw;
            }
        }

        /// <summary>
        /// Get IVRFlow by flow code
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="flowCode">flow code</param>
        /// <returns>IVRflow details</returns>
        [HttpGet("by-code/{flowCode}")]
        [ProducesResponseType(typeof(ApiResponse<IVRFlowsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<IVRFlowsdto>>> GetIVRFlowByCode(
            [FromQuery] Guid tenantId,
            string flowCode)
        {
            try
            {
                _logger.LogInformation("Fetching iVRFlow with code {flowCode} for tenant {TenantId}",
                    flowCode, tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var iVRFlows = await _unitOfWork.IVRFlows.GetByIVRFlowCodeAsync(tenantId, flowCode);

                if (iVRFlows == null)
                {
                    _logger.LogWarning("IVRFlow with code {flowCode} not found for tenant {TenantId}",
                        flowCode, tenantId);
                    return NotFound(new ApiResponse<object>($"IVRFlow with code {flowCode} not found"));
                }

                var iVRFlowsdto = _mapper.Map<IVRFlowsdto>(iVRFlows);


                _logger.LogInformation("Successfully retrieved iVRFlows with code {FlowCode}", flowCode);

                return Ok(new ApiResponse<IVRFlowsdto>(iVRFlowsdto, "IVRFlows retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching iVRFLows with code {FlowCode}", flowCode);
                throw;
            }
        }

        /// <summary>
        /// Create a new iVRFlows
        /// </summary>
        /// <param name="createIVRFlowsdto">IVRflow creation data</param>
        /// <returns>Created iVRFlows details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<IVRFlowsdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<IVRFlowsdto>>> CreateIVRFlows([FromBody] CreateIVRFlowsdto createIVRFlowsdto)
        {
            try
            {
                _logger.LogInformation("Creating new iVRFlows with code {FlowCode} for tenant {TenantId}",
                    createIVRFlowsdto.FlowCode, createIVRFlowsdto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createIVRFlowsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for iVRFlow creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Check if iVRFlow code is unique
                var isUnique = await _unitOfWork.IVRFlows.IsIVRFlowCodeUniqueAsync(
                    createIVRFlowsdto.TenantId, createIVRFlowsdto.FlowCode);

                if (!isUnique)
                {
                    _logger.LogWarning("flow code {FlowCode} already exists for tenant {TenantId}",
                        createIVRFlowsdto.FlowCode, createIVRFlowsdto.TenantId);
                    return BadRequest(new ApiResponse<object>("Flow code already exists"));
                }

                // Map DTO to entity
                var iVRFlows = _mapper.Map<IVRFlows>(createIVRFlowsdto);
                iVRFlows.IVRFLowId = Guid.NewGuid();
                iVRFlows.CreatedDate = DateTime.UtcNow;

                // Add to database
                 await _unitOfWork.IVRFlows.AddAsync(iVRFlows);
                await _unitOfWork.SaveChangesAsync();

                var iVRFlowsDto = _mapper.Map<IVRFlowsdto>(iVRFlows);

                _logger.LogInformation("Successfully created iVRFLows {IVRFlowId} with code {FlowCode}",
                    iVRFlows.IVRFLowId, iVRFlows.FlowCode);

                return CreatedAtAction(
                    nameof(GetIVRFlowsById),
                    new { id = iVRFlows.IVRFLowId },
                    new ApiResponse<IVRFlowsdto>(iVRFlowsDto, "IVRFlows created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating iVRFlow");
                throw;
            }
        }

        /// <summary>
        /// Update an existing iVRFlow
        /// </summary>
        /// <param name="id">IVRFlows ID</param>
        /// <param name="updateIVRFlowsdto">IVRFlows update data</param>
        /// <returns>Updated iVRFlows details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<IVRFlowsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<IVRFlowsdto>>> UpdateIVRFlows(
            Guid id,
            [FromBody] UpdateIVRFlowsdto updateIVRFlowsdto)
        {
            try
            {
                _logger.LogInformation("Updating iVRFlow {IVRFlowId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateIVRFlowsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for iVRFlow update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing IVRFlow
                var existingIVRFlows = await _unitOfWork.IVRFlows.GetByIdAsync(id);
                if (existingIVRFlows == null)
                {
                    _logger.LogWarning("IVRFlow {IVRFlowId} not found", id);
                    return NotFound(new ApiResponse<object>($"IVRFlow with ID {id} not found"));
                }

                // Update only provided fields
               

                _unitOfWork.IVRFlows.Update(existingIVRFlows);
                await _unitOfWork.SaveChangesAsync();

                var iVRFlowsDto = _mapper.Map<IVRFlowsdto>(existingIVRFlows);

                _logger.LogInformation("Successfully updated iVRFlows {IVRFlowId}", id);

                return Ok(new ApiResponse<IVRFlowsdto>(iVRFlowsDto, "IVRFlow updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating iVRFlows {IVRFlowId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a iVRFlow (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">IVRflow ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteIVRFlows(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting iVRFlows {IVRFlowId}", id);

                var iVRFlows = await _unitOfWork.IVRFlows.GetByIdAsync(id);
                if (iVRFlows == null)
                {
                    _logger.LogWarning("IVRFlows {IVrflowId} not found", id);
                    return NotFound(new ApiResponse<object>($"IVRFlow with ID {id} not found"));
                }

                // Soft delete
               

                _unitOfWork.IVRFlows.Update(iVRFlows);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted iVRFlow {IVRFlowId}", id);

                return Ok(new ApiResponse<object>(null, "IVRFlows deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting iVRFlow {IVRFlowId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search iVRFlows by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching iVRFlows</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<IVRFlowsdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<IVRFlowsdto>>>> SearchIVRFlows(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching iVRFlows for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var iVRFlows = await _unitOfWork.IVRFlows.SearchIVRFlowsAsync(tenantId, searchTerm);
                var iVRFlowsDtos = _mapper.Map<List<IVRFlowsdto>>(iVRFlows);

                _logger.LogInformation("Found {Count} iVRFlows matching search term", iVRFlowsDtos.Count);

                return Ok(new ApiResponse<List<IVRFlowsdto>>(iVRFlowsDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching iVRFlows");
                throw;
            }
        }

        /// <summary>
        /// Get active iVRFlow for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active iVRFlows</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<IVRFlowsdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<IVRFlowsdto>>>> GetActiveIVRFlows([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active iVRFlows for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var iVRFlows = await _unitOfWork.IVRFlows.GetActiveIVRFlowsAsync(tenantId);
                var iVRFlowsDtos = _mapper.Map<List<IVRFlowsdto>>(iVRFlows);

                _logger.LogInformation("Successfully retrieved {Count} active iVRFlows", iVRFlowsDtos.Count);

                return Ok(new ApiResponse<List<IVRFlowsdto>>(iVRFlowsDtos, "Active iVRFlows retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active iVRFlows for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}