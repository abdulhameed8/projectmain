using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.AuditLogs;

using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// AuditLogs management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AuditLogsController> _logger;
        private readonly IValidator<CreateAuditLogsdto> _createValidator;
        private readonly IValidator<UpdateAuditLogsdto> _updateValidator;

        public AuditLogsController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AuditLogsController> logger,
            IValidator<CreateAuditLogsdto> createValidator,
            IValidator<UpdateAuditLogsdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all AuditLogs with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of auditLogs</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<AuditLogsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<AuditLogsdto>>> GetAuditLogs(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching auditLogs for auditLogs {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (auditLogs, totalCount) = await _unitOfWork.AuditLogs.GetAuditLogsPagedAsync(
                    tenantId, searchTerm, pageNumber, pageSize);

                var auditLogsDtos = _mapper.Map<List<AuditLogsdto>>(auditLogs);

                var response = new PagedResponse<AuditLogsdto>(auditLogsDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} auditLogs out of {TotalCount} for auditLogs {TenantId}",
                    auditLogsDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching auditLogs for auditLogs {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get auditLogs by ID
        /// </summary>
        /// <param name="id">AuditLogs ID</param>
        /// <returns>AuditLogs details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AuditLogsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]

        public async Task<ActionResult<ApiResponse<AuditLogsdto>>> GetAuditLogsById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching auditLogs with ID: {AuditLogsId}", id);

                var auditLogs = await _unitOfWork.AuditLogs.GetByIdAsync(id);

                if (auditLogs == null)
                {
                    _logger.LogWarning("AuditLogs with ID {AuditLogsId} not found", id);
                    return NotFound(new ApiResponse<object>($"AuditLogs with ID {id} not found"));
                }

                var auditLogsdto = _mapper.Map<AuditLogsdto>(auditLogs);

                _logger.LogInformation("Successfully retrieved auditLogs {AuditLogsId}", id);

                return Ok(new ApiResponse<AuditLogsdto>(auditLogsdto, "AuditLogs retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching auditLogs {AuditLogsId}", id);
                throw;
            }
        }

        /// <summary>
        /// Create a new auditLogs
        /// </summary>
        /// <param name="createAuditLogsdto">AuditLogs creation data</param>
        /// <returns>Created auditLogs details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AuditLogsdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<AuditLogsdto>>> CreateAuditLogs([FromBody] CreateAuditLogsdto createAuditLogsdto)
        {
            try
            {
                _logger.LogInformation("Creating new auditLogs  for tenant {TenantId}",
                     createAuditLogsdto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createAuditLogsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for auditLogs creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

               

                // Map DTO to entity
                var auditLogs = _mapper.Map<AuditLogs>(createAuditLogsdto);
                auditLogs.AuditLogId = Guid.NewGuid();
                auditLogs.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.AuditLogs.AddAsync(auditLogs);
                await _unitOfWork.SaveChangesAsync();

                var auditLogsdto = _mapper.Map<AuditLogsdto>(auditLogs);

                _logger.LogInformation("Successfully created auditLogs {AiditLogsId}",
                    auditLogs.AuditLogId );

                return CreatedAtAction(
                    nameof(GetAuditLogsById),
                    new { id = auditLogs.AuditLogId },
                    new ApiResponse<AuditLogsdto>(auditLogsdto, "AuditLogs created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating auditLogs");
                throw;
            }
        }

        /// <summary>
        /// Update an existing auditLogs
        /// </summary>
        /// <param name="id">AuditLogs ID</param>
        /// <param name="updateAuditLogsdto">AuditLogs update data</param>
        /// <returns>Updated auditLogs details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AuditLogsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<AuditLogsdto>>> UpdateAuditLogs(
            Guid id,
            [FromBody] UpdateAuditLogsdto updateAuditLogsdto)
        {
            try
            {
                _logger.LogInformation("Updating auditLogs {AiditLogId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateAuditLogsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for auditLogs update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing auditLogs
                var existingauditLogs = await _unitOfWork.AuditLogs.GetByIdAsync(id);
                if (existingauditLogs == null)
                {
                    _logger.LogWarning("AuditLogs {AuditLogsId} not found", id);
                    return NotFound(new ApiResponse<object>($"AuditLogs with ID {id} not found"));
                }

                // Update only provided fields
               

                if (!string.IsNullOrWhiteSpace(updateAuditLogsdto.EntityName))
                    existingauditLogs.EntityName = updateAuditLogsdto.EntityName;

                
                _unitOfWork.AuditLogs.Update(existingauditLogs);
                await _unitOfWork.SaveChangesAsync();

                var auditLogsdto = _mapper.Map<AuditLogsdto>(existingauditLogs);

                _logger.LogInformation("Successfully updated auditLogs {AuditLogsId}", id);

                return Ok(new ApiResponse<AuditLogsdto>(auditLogsdto, "AuditLogs updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating auditLogs {AuditLogId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search auditLogs by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching auditLogss</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<AuditLogsdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<AuditLogsdto>>>> SearchAuditLogs(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching auditLogs for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var auditLogs = await _unitOfWork.AuditLogs.SearchAuditLogsAsync(tenantId, searchTerm);
                var auditLogsDtos = _mapper.Map<List<AuditLogsdto>>(auditLogs);

                _logger.LogInformation("Found {Count} auditLogs matching search term", auditLogsDtos.Count);

                return Ok(new ApiResponse<List<AuditLogsdto>>(auditLogsDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching auditLogs");
                throw;
            }
        }

        /// <summary>
        /// Get active auditLogs for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active auditLogs</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<AuditLogsdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<AuditLogsdto>>>> GetActiveAuditLogs([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active auditLogs for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var auditLogs = await _unitOfWork.AuditLogs.GetActiveAuditLogsAsync(tenantId);
                var auditLogsDtos = _mapper.Map<List<AuditLogsdto>>(auditLogs);

                _logger.LogInformation("Successfully retrieved {Count} active auditLogs", auditLogsDtos.Count);

                return Ok(new ApiResponse<List<AuditLogsdto>>(auditLogsDtos, "Active auditLogs retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active auditLogs for tenant {TenantId}", tenantId);
                throw;
            }
        }




    }
}
