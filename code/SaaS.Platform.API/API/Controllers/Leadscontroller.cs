using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Customer;
using SaaS.Platform.API.Application.DTOs.Leads;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// Customer management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class LeadsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<LeadsController> _logger;
        private readonly IValidator<CreateLeadsdto> _createValidator;
        private readonly IValidator<UpdateLeadsdto> _updateValidator;

        public LeadsController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<LeadsController> logger,
            IValidator<CreateLeadsdto> createValidator,
            IValidator<UpdateLeadsdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all leads with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of leads</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<Leadsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<Leadsdto>>> GetLeads(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching leads for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (leads, totalCount) = await _unitOfWork.Leads.GetLeadsPagedAsync(
                    tenantId, searchTerm,pageNumber, pageSize);

                var leadsDtos = _mapper.Map<List<Leadsdto>>(leads);

                var response = new PagedResponse<Leadsdto>(leadsDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} leads out of {TotalCount} for tenant {TenantId}",
                    leadsDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching cleads for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get leads by ID
        /// </summary>
        /// <param name="id">Leads ID</param>
        /// <returns>Leads details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Leadsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Leadsdto>>> GetLeadsById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching leads with ID: {LeadId}", id);

                var leads = await _unitOfWork.Leads.GetByIdAsync(id);

                if (leads == null)
                {
                    _logger.LogWarning("Leads with ID {LeadId} not found", id);
                    return NotFound(new ApiResponse<object>($"Leads with ID {id} not found"));
                }

                var leadsDto = _mapper.Map<Leadsdto>(leads);

                _logger.LogInformation("Successfully retrieved leads {LeadsId}", id);

                return Ok(new ApiResponse<Leadsdto>(leadsDto, "Leads retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching leads {LeadId}", id);
                throw;
            }
        }

        /// <summary>
        /// Get leads by lead code
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="customerCode">Leads code</param>
        /// <returns>Leads details</returns>
        [HttpGet("by-code/{leadCode}")]
        [ProducesResponseType(typeof(ApiResponse<Leadsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Leadsdto>>> GetLeadsByCode(
            [FromQuery] Guid tenantId,
            string leadCode)
        {
            try
            {
                _logger.LogInformation("Fetching lead with code {LeadCode} for tenant {TenantId}",
                    leadCode, tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var leads = await _unitOfWork.Leads.GetByLeadsCodeAsync(tenantId, leadCode);

                if (leads == null)
                {
                    _logger.LogWarning("Leads with code {LeadsCode} not found for tenant {TenantId}",
                        leadCode, tenantId);
                    return NotFound(new ApiResponse<object>($"Lead with code {leadCode} not found"));
                }

                var leadsDto = _mapper.Map<Leadsdto>(leads);

                _logger.LogInformation("Successfully retrieved leads with code {LeadCode}", leadCode);

                return Ok(new ApiResponse<Leadsdto>(leadsDto, "Leads retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching leads with code {LeadCode}", leadCode);
                throw;
            }
        }

        /// <summary>
        /// Create a new lead
        /// </summary>
        /// <param name="createLeadsdto">Leads creation data</param>
        /// <returns>Created leads details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Leadsdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Leadsdto>>> CreateLeads([FromBody] CreateLeadsdto createLeadsdto)
        {
            try
            {
                _logger.LogInformation("Creating new leads with code {LeadCode} for tenant {TenantId}",
                    createLeadsdto.LeadsCode, createLeadsdto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createLeadsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Check if leads code is unique
                var isUnique = await _unitOfWork.Leads.IsLeadsCodeUniqueAsync(
                    createLeadsdto.TenantId, createLeadsdto.LeadsCode);

                if (!isUnique)
                {
                    _logger.LogWarning("Leads code {LeadCode} already exists for tenant {TenantId}",
                        createLeadsdto.LeadsCode, createLeadsdto.TenantId);
                    return BadRequest(new ApiResponse<object>("Lead code already exists"));
                }

                // Map DTO to entity
                var leads = _mapper.Map<Leads>(createLeadsdto);
                leads.LeadId = Guid.NewGuid();
                leads.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.Leads.AddAsync(leads);
                await _unitOfWork.SaveChangesAsync();

                var leadsDto = _mapper.Map<Leadsdto>(leads);

                _logger.LogInformation("Successfully created leads {LeadId} with code {LeadCode}",
                    leads.LeadId, leads.LeadsCode);

                return CreatedAtAction(
                    nameof(GetLeadsById),
                    new { id = leads.LeadId },
                    new ApiResponse<Leadsdto>(leadsDto, "Leads created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating customer");
                throw;
            }
        }

        /// <summary>
        /// Update an existing leads
        /// </summary>
        /// <param name="id">Leads ID</param>
        /// <param name="updateLeadsdto">Leads update data</param>
        /// <returns>Updated leads details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Leadsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Leadsdto>>> UpdateLeads(
            Guid id,
            [FromBody] UpdateLeadsdto updateLeadsdto)
        {
            try
            {
                _logger.LogInformation("Updating leads {LeadId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateLeadsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing leads
                var existingLeads = await _unitOfWork.Leads.GetByIdAsync(id);
                if (existingLeads == null)
                {
                    _logger.LogWarning("Leads {LeadId} not found", id);
                    return NotFound(new ApiResponse<object>($"Lead with ID {id} not found"));
                }

                // Update only provided fields
                if (!string.IsNullOrWhiteSpace(updateLeadsdto.FirstName))
                    existingLeads.FirstName = updateLeadsdto.FirstName;

                if (!string.IsNullOrWhiteSpace(updateLeadsdto.LastName))
                    existingLeads.LastName = updateLeadsdto.LastName;

                if (!string.IsNullOrWhiteSpace(updateLeadsdto.CompanyName))
                    existingLeads.CompanyName = updateLeadsdto.CompanyName;

                if (!string.IsNullOrWhiteSpace(updateLeadsdto.Email))
                    existingLeads.Email = updateLeadsdto.Email;

                if (!string.IsNullOrWhiteSpace(updateLeadsdto.Phone))
                    existingLeads.Phone = updateLeadsdto.Phone;

                if (updateLeadsdto.AssignedUserId.HasValue)
                    existingLeads.AssignedUserId = updateLeadsdto.AssignedUserId;



                existingLeads.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Leads.Update(existingLeads);
                await _unitOfWork.SaveChangesAsync();

                var leadsDto = _mapper.Map<Leadsdto>(existingLeads);

                _logger.LogInformation("Successfully updated leads {LeadId}", id);

                return Ok(new ApiResponse<Leadsdto>(leadsDto, "Leads updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating leads {LeadId}", id);
                throw;
            }
        }
        /// <summary>
        /// Delete a leads (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Leads ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteLeads(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting leads {LeadsId}", id);

                var leads = await _unitOfWork.Leads.GetByIdAsync(id);
                if (leads == null)
                {
                    _logger.LogWarning("Leads {LeadId} not found", id);
                    return NotFound(new ApiResponse<object>($"Leads with ID {id} not found"));
                }

                // Soft delete
                
                leads.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Leads.Update(leads);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted leads {LeadId}", id);

                return Ok(new ApiResponse<object>(null, "Leads deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting leads {LeadsId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search leads by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching leads</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<Leadsdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Leadsdto>>>> SearchLeads(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching Leads for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var leads = await _unitOfWork.Leads.SearchLeadsAsync(tenantId, searchTerm);
                var leadsDtos = _mapper.Map<List<Leadsdto>>(leads);

                _logger.LogInformation("Found {Count} leads matching search term", leadsDtos.Count);

                return Ok(new ApiResponse<List<Leadsdto>>(leadsDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching leads");
                throw;
            }
        }


        /// <summary>
        /// Get active leads for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active leads</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<Leadsdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Leadsdto>>>> GetActiveLeads([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active leads for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var leads = await _unitOfWork.Leads.GetActiveLeadsAsync(tenantId);
                var leadsDtos = _mapper.Map<List<Leadsdto>>(leads);

                _logger.LogInformation("Successfully retrieved {Count} active leads", leadsDtos.Count);

                return Ok(new ApiResponse<List<Leadsdto>>(leadsDtos, "Active leads retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active leads for tenant {TenantId}", tenantId);
                throw;
            }
        }




    }
}
