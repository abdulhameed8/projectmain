using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.CallRecord;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// Callrecord management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class CallRecordController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CallRecordController> _logger;
        private readonly IValidator<CreateCallRecorddto> _createValidator;
        private readonly IValidator<UpdateCallRecorddto> _updateValidator;

        public CallRecordController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CallRecordController> logger,
            IValidator<CreateCallRecorddto> createValidator,
            IValidator<UpdateCallRecorddto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all callRecord with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="status">Call status filter</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of callRecord</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<CallRecorddto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<CallRecorddto>>> GetCallRecords(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? status = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching callRecords for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (callRecords, totalCount) = await _unitOfWork.CallRecord.GetCallRecordsPagedAsync(
                    tenantId, searchTerm, status,  pageNumber, pageSize);

                var callRecordDtos = _mapper.Map<List<CallRecorddto>>(callRecords);

                var response = new PagedResponse<CallRecorddto>(callRecordDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} customers out of {TotalCount} for tenant {TenantId}",
                    callRecordDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching callRecords for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get callRecord by ID
        /// </summary>
        /// <param name="id">CallRecord ID</param>
        /// <returns>CallRecord details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CallRecorddto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CallRecorddto>>> GetCallRecordById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching callRecord with ID: {CallId}", id);

                var callRecord = await _unitOfWork.CallRecord.GetByIdAsync(id);

                if (callRecord == null)
                {
                    _logger.LogWarning("CallRecord with ID {CallId} not found", id);
                    return NotFound(new ApiResponse<object>($"CallRecord with ID {id} not found"));
                }

                var callRecordDto = _mapper.Map<CallRecorddto>(callRecord);

                _logger.LogInformation("Successfully retrieved callRecord {CallId}", id);

                return Ok(new ApiResponse<CallRecorddto>(callRecordDto, "Call retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching callRecord {CallId}", id);
                throw;
            }
        }

        
        /// <summary>
        /// Create a new callRecord
        /// </summary>
        /// <param name="createCallRecordto">CallRecord creation data</param>
        /// <returns>Created callRecord details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CallRecorddto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<CallRecorddto>>> CreateCallRecord([FromBody] CreateCallRecorddto createCallRecorddto)
        {
            try
            {
                

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createCallRecorddto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

               

                // Map DTO to entity
                var callRecord = _mapper.Map<CallRecord>(createCallRecorddto);
                callRecord.CallId = Guid.NewGuid();
                callRecord.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.CallRecord.AddAsync(callRecord);
                await _unitOfWork.SaveChangesAsync();

                var callRecordDto = _mapper.Map<CallRecorddto>(callRecord);

                _logger.LogInformation("Successfully created callRecord {CallId}",
                    callRecord.CallId);

                return CreatedAtAction(
                    nameof(GetCallRecordById),
                    new { id = callRecord.CallId },
                    new ApiResponse<CallRecorddto>(callRecordDto, "CallRecord created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating callRecord");
                throw;
            }
        }

        /// <summary>
        /// Update an existing callRecord
        /// </summary>
        /// <param name="id">Call ID</param>
        /// <param name="updateCallRecorddto">CallRecord update data</param>
        /// <returns>Updated callRecord details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CallRecorddto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CallRecorddto>>> UpdateCallRecord(
            Guid id,
            [FromBody] UpdateCallRecorddto updateCallRecorddto)
        {
            try
            {
                _logger.LogInformation("Updating callRecord {CallId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateCallRecorddto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing callRecord
                var existingCallRecord = await _unitOfWork.CallRecord.GetByIdAsync(id);
                if (existingCallRecord == null)
                {
                    _logger.LogWarning("CallRecord {CallId} not found", id);
                    return NotFound(new ApiResponse<object>($"CallRecord with ID {id} not found"));
                }

                // Update only provided fields
                 if (!string.IsNullOrWhiteSpace(updateCallRecorddto.CallStatus))
                    existingCallRecord.CallStatus = updateCallRecorddto.CallStatus;

                if (updateCallRecorddto.AgentUserId.HasValue)
                    existingCallRecord.AgentUserId = updateCallRecorddto.AgentUserId;

               

                _unitOfWork.CallRecord.Update(existingCallRecord);
                await _unitOfWork.SaveChangesAsync();

                var callRecordDto = _mapper.Map<CallRecorddto>(existingCallRecord);

                _logger.LogInformation("Successfully updated callRecord {CallId}", id);

                return Ok(new ApiResponse<CallRecorddto>(callRecordDto, "CallRecord updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating callRecord {CallId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a callRecord (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Call ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteCallRecord(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting callRecord {CallId}", id);

                var callRecord = await _unitOfWork.CallRecord.GetByIdAsync(id);
                if (callRecord == null)
                {
                    _logger.LogWarning("CallRecord {CallId} not found", id);
                    return NotFound(new ApiResponse<object>($"CallRecord with ID {id} not found"));
                }

                // Soft delete
                callRecord.CallStatus = "Inactive";

                _unitOfWork.CallRecord.Update(callRecord);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted callRecord {CallId}", id);

                return Ok(new ApiResponse<object>(null, "CallRecord deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting callRecord {CallId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search callRecord by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching callRecord</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<CallRecorddto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<CallRecorddto>>>> SearchCallRecords(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching calRecords for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var callRecords = await _unitOfWork.CallRecord.SearchCallRecordsAsync(tenantId, searchTerm);
                var callRecordDtos = _mapper.Map<List<CallRecorddto>>(callRecords);

                _logger.LogInformation("Found {Count} callRecord matching search term", callRecordDtos.Count);

                return Ok(new ApiResponse<List<CallRecorddto>>(callRecordDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching callRecords");
                throw;
            }
        }

        /// <summary>
        /// Get active callRecord for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active callRecords</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<CallRecorddto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<CallRecorddto>>>> GetActiveCallRecords([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active callRecords for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var callRecords = await _unitOfWork.CallRecord.GetActiveCallRecordsAsync(tenantId);
                var callRecordDtos = _mapper.Map<List<CallRecorddto>>(callRecords);

                _logger.LogInformation("Successfully retrieved {Count} active callRecords", callRecordDtos.Count);

                return Ok(new ApiResponse<List<CallRecorddto>>(callRecordDtos, "Active callRecords retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active callRecords for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}
