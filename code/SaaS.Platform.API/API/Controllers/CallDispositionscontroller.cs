using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.CallDispositions;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// CallDisposition management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class CallDispositionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CallDispositionController> _logger;
        private readonly IValidator<CreateCallDispositiondto> _createValidator;
        private readonly IValidator<UpdateCallDispositiondto> _updateValidator;

        public CallDispositionController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CallDispositionController> logger,
            IValidator<CreateCallDispositiondto> createValidator,
            IValidator<UpdateCallDispositiondto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all calldispositions with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of callDispositions</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<CallDispositiondto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<CallDispositiondto>>> GetCalldispositions(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching callDisposition for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (callDispositions, totalCount) = await _unitOfWork.CallDispositions.GetCallDispositionsPagedAsync(
                    tenantId, searchTerm, pageNumber, pageSize);

                var callDispositionDtos = _mapper.Map<List<CallDispositiondto>>(callDispositions);

                var response = new PagedResponse<CallDispositiondto>(callDispositionDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} callDispositions out of {TotalCount} for tenant {TenantId}",
                    callDispositionDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching callDispositions for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get callDisposition by ID
        /// </summary>
        /// <param name="id">CallDisposition ID</param>
        /// <returns>CCallDisposition details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CallDispositiondto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CallDispositiondto>>> GetCallDispositionById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching callDisposition with ID: {CallDispositionId}", id);

                var callDisposition = await _unitOfWork.CallDispositions.GetByIdAsync(id);

                if (callDisposition == null)
                {
                    _logger.LogWarning("CallDisposition with ID {CallDispositionId} not found", id);
                    return NotFound(new ApiResponse<object>($"CallDisposition with ID {id} not found"));
                }

                var callDispositionDto = _mapper.Map<CallDispositiondto>(callDisposition);

                _logger.LogInformation("Successfully retrieved callDisposition {CallDispositionId}", id);

                return Ok(new ApiResponse<CallDispositiondto>(callDispositionDto, "CallDisposition retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching callDisposition {CallDispositionId}", id);
                throw;
            }
        }

        /// <summary>
        /// Get callDisposition by disposition code
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="dispositionCode">Disposition code</param>
        /// <returns>CallDisposition details</returns>
        [HttpGet("by-code/{dispositionCode}")]
        [ProducesResponseType(typeof(ApiResponse<CallDispositiondto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CallDispositiondto>>> GetCallDispositionByCode(
            [FromQuery] Guid tenantId,
            string dispositionCode)
        {
            try
            {
                _logger.LogInformation("Fetching callDisposition with code {DispositionCode} for tenant {TenantId}",
                    dispositionCode, tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var callDisposition = await _unitOfWork.CallDispositions.GetByDispositionCodeAsync(tenantId, dispositionCode);

                if (callDisposition == null)
                {
                    _logger.LogWarning("CallDisposition with code {DispositionCode} not found for tenant {TenantId}",
                        dispositionCode, tenantId);
                    return NotFound(new ApiResponse<object>($"CallDisposition with code {dispositionCode} not found"));
                }

                var callDispositionDto = _mapper.Map<CallDispositiondto>(callDisposition);

                _logger.LogInformation("Successfully retrieved callDisposition with code {DispositionCode}", dispositionCode);

                return Ok(new ApiResponse<CallDispositiondto>(callDispositionDto, "CallDisposition retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching callDisposition with code {DispositionCode}", dispositionCode);
                throw;
            }
        }

        /// <summary>
        /// Create a new callDispositions
        /// </summary>
        /// <param name="createCallDispositiondto">CallDisposition creation data</param>
        /// <returns>Created callDisposition details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CallDispositiondto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<CallDispositiondto>>> CreateCallDisposition([FromBody] CreateCallDispositiondto createCallDispositiondto)
        {
            try
            {
                _logger.LogInformation("Creating new callDisposition with code {DispositionCode} for tenant {TenantId}",
                    createCallDispositiondto.DispositionCode, createCallDispositiondto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createCallDispositiondto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Check if disposition code is unique
                var isUnique = await _unitOfWork.CallDispositions.IsDispositionCodeUniqueAsync(
                    createCallDispositiondto.TenantId, createCallDispositiondto.DispositionCode);

                if (!isUnique)
                {
                    _logger.LogWarning("Disposition code {DispositionCode} already exists for tenant {TenantId}",
                        createCallDispositiondto.DispositionCode, createCallDispositiondto.TenantId);
                    return BadRequest(new ApiResponse<object>("disposition code already exists"));
                }

                // Map DTO to entity
                var callDisposition = _mapper.Map<CallDispositions>(createCallDispositiondto);
                callDisposition.CallDispositionId = Guid.NewGuid();
                callDisposition.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.CallDispositions.AddAsync(callDisposition);
                await _unitOfWork.SaveChangesAsync();

                var callDispositionDto = _mapper.Map<CallDispositiondto>(callDisposition);

                _logger.LogInformation("Successfully created callDisposition {CallDispositionId} with code {DispositionCode}",
                    callDisposition.CallDispositionId, callDisposition.DispositionCode);

                return CreatedAtAction(
                    nameof(GetCallDispositionById),
                    new { id = callDisposition.CallDispositionId },
                    new ApiResponse<CallDispositiondto>(callDispositionDto, "CallDispositions created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating callDispositions");
                throw;
            }
        }

        /// <summary>
        /// Update an existing callDispositions
        /// </summary>
        /// <param name="id">CallDisposition ID</param>
        /// <param name="updateCallDispositiondto">CallDispositions update data</param>
        /// <returns>Updated callDispositions details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CallDispositiondto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CallDispositiondto>>> UpdateCallDisposition(
            Guid id,
            [FromBody] UpdateCallDispositiondto updateCallDispositiondto)
        {
            try
            {
                _logger.LogInformation("Updating callDisposition {CallDispositionId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateCallDispositiondto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing callDispositions
                var existingCallDisposition = await _unitOfWork.CallDispositions.GetByIdAsync(id);
                if (existingCallDisposition == null)
                {
                    _logger.LogWarning("CallDisposition {CallDisppsitionId} not found", id);
                    return NotFound(new ApiResponse<object>($"CallDisposition with ID {id} not found"));
                }

                // Update only provided fields

                if (!string.IsNullOrWhiteSpace(updateCallDispositiondto.DispositionName))
                    existingCallDisposition.DispositionName = updateCallDispositiondto.DispositionName;

                if (updateCallDispositiondto.IsActive.HasValue)
                    existingCallDisposition.IsActive = updateCallDispositiondto.IsActive.Value;


                _unitOfWork.CallDispositions.Update(existingCallDisposition);
                await _unitOfWork.SaveChangesAsync();

                var calllDispositionDto = _mapper.Map<CallDispositiondto>(existingCallDisposition);

                _logger.LogInformation("Successfully updated callDisposition {CallDispositionId}", id);

                return Ok(new ApiResponse<CallDispositiondto>(calllDispositionDto, "CallDisposition updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating callDisposition {CallDispositionId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a calldispositions (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Calldisposition ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteCalldisposition(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting callDisposition {CallDispositionId}", id);

                var callDisposition = await _unitOfWork.CallDispositions.GetByIdAsync(id);
                if (callDisposition == null)
                {
                    _logger.LogWarning("Calldisposition {CallDispositionId} not found", id);
                    return NotFound(new ApiResponse<object>($"CallDisposition with ID {id} not found"));
                }

                // Soft delete
                callDisposition.IsActive = false;

                _unitOfWork.CallDispositions.Update(callDisposition);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted callDisposition {CalldispositionId}", id);

                return Ok(new ApiResponse<object>(null, "Calldisposition deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting calldisposition {CallDispositionId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search calldisposition by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching calldisposition</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<CallDispositiondto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<CallDispositiondto>>>> SearchCallDispositions(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching callDispositions for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var callDisposition = await _unitOfWork.CallDispositions.SearchCallDispositionsAsync(tenantId, searchTerm);
                var callDispositionDtos = _mapper.Map<List<CallDispositiondto>>(callDisposition);

                _logger.LogInformation("Found {Count} callDispositions matching search term", callDispositionDtos.Count);

                return Ok(new ApiResponse<List<CallDispositiondto>>(callDispositionDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching callDispositions");
                throw;
            }
        }

        /// <summary>
        /// Get active callDispositions for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active callDispositions</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<CallDispositiondto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<CallDispositiondto>>>> GetActiveCallDispositions([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active callDispositions for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var callDisposition = await _unitOfWork.CallDispositions.GetActiveCallDispositionsAsync(tenantId);
                var callDispositionDtos = _mapper.Map<List<CallDispositiondto>>(callDisposition);

                _logger.LogInformation("Successfully retrieved {Count} active callDispositions", callDispositionDtos.Count);

                return Ok(new ApiResponse<List<CallDispositiondto>>(callDispositionDtos, "Active callDispositions retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active callDispositions for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}
