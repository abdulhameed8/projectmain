using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Opportunities;
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
    public class OpportunityController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OpportunityController> _logger;
        private readonly IValidator<CreateOpportunitydto> _createValidator;
        private readonly IValidator<UpdateOpportunitydto> _updateValidator;

        public OpportunityController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<OpportunityController> logger,
            IValidator<CreateOpportunitydto> createValidator,
            IValidator<UpdateOpportunitydto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all opportunity with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of opportunities</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<Opportunitydto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<Opportunitydto>>> GetOpportunity(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching opportunity for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (opportunities, totalCount) = await _unitOfWork.Opportunity.GetOpportunityPagedAsync(
                    tenantId, searchTerm, pageNumber, pageSize);

                var opportunityDtos = _mapper.Map<List<Opportunitydto>>(opportunities);

                var response = new PagedResponse<Opportunitydto>(opportunityDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} opportunities out of {TotalCount} for tenant {TenantId}",
                    opportunityDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching opportunities for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get opportunity by ID
        /// </summary>
        /// <param name="id">Opportunity ID</param>
        /// <returns>Opportunity details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Opportunitydto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Opportunitydto>>> GetOpportunityById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching opportunity with ID: {OPportunityId}", id);

                var opportunity = await _unitOfWork.Opportunity.GetByIdAsync(id);

                if (opportunity == null)
                {
                    _logger.LogWarning("Opportunity with ID {OpportunityId} not found", id);
                    return NotFound(new ApiResponse<object>($"Opportunity with ID {id} not found"));
                }

                var opportunityDto = _mapper.Map<Opportunitydto>(opportunity);

                _logger.LogInformation("Successfully retrieved opportunity {OPportunityId}", id);

                return Ok(new ApiResponse<Opportunitydto>(opportunityDto, "Opportunity retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching opportunity {OpportunityId}", id);
                throw;
            }
        }

        /// <summary>
        /// Get opportunity by opportunity code
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="customerCode">Opportunity code</param>
        /// <returns>Opportunity details</returns>
        [HttpGet("by-code/{opportunityCode}")]
        [ProducesResponseType(typeof(ApiResponse<Opportunitydto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Opportunitydto>>> GetOpportunityByCode(
            [FromQuery] Guid tenantId,
            string opportunityCode)
        {
            try
            {
                _logger.LogInformation("Fetching opportunity with code {OpportunityCode} for tenant {TenantId}",
                    opportunityCode, tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var opportunity = await _unitOfWork.Opportunity.GetByOpportunityCodeAsync(tenantId, opportunityCode);

                if (opportunity == null)
                {
                    _logger.LogWarning("Opportunity with code {OpportunityCode} not found for tenant {TenantId}",
                        opportunityCode, tenantId);
                    return NotFound(new ApiResponse<object>($"Opportunity with code {opportunityCode} not found"));
                }

                var opportunityDto = _mapper.Map<Opportunitydto>(opportunity);

                _logger.LogInformation("Successfully retrieved opprtunity with code {OpportunityCode}", opportunityCode);

                return Ok(new ApiResponse<Opportunitydto>(opportunityDto, "Opporotunity retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching opportunity with code {OpportunityCode}", opportunityCode);
                throw;
            }
        }


        /// <summary>
        /// Create a new opportunity
        /// </summary>
        /// <param name="createopportunitydto">Opportunity creation data</param>
        /// <returns>Created opportunity details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Opportunitydto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Opportunitydto>>> CreateOpportunity([FromBody] CreateOpportunitydto createOpportunitydto)
        {
            try
            {
                _logger.LogInformation("Creating new opportunity with code {OpportunityCode} for tenant {TenantId}",
                    createOpportunitydto.OpportunityCode, createOpportunitydto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createOpportunitydto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Check if opportunity code is unique
                var isUnique = await _unitOfWork.Opportunity.IsOpportunityCodeUniqueAsync(
                    createOpportunitydto.TenantId, createOpportunitydto.OpportunityCode);

                if (!isUnique)
                {
                    _logger.LogWarning("Opportunity code {OpportunityCode} already exists for tenant {TenantId}",
                        createOpportunitydto.OpportunityCode, createOpportunitydto.TenantId);
                    return BadRequest(new ApiResponse<object>("Opportunity code already exists"));
                }

                // Map DTO to entity
                var opportunity = _mapper.Map<Opportunities>(createOpportunitydto);
                opportunity.OpportunityId = Guid.NewGuid();
                opportunity.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.Opportunity.AddAsync(opportunity);
                await _unitOfWork.SaveChangesAsync();

                var opportunityDto = _mapper.Map<Opportunitydto>(opportunity);

                _logger.LogInformation("Successfully created opportunity {OpportunityId} with code {OpportunityCode}",
                    opportunity.OpportunityId, opportunity.OpportunityCode);

                return CreatedAtAction(
                    nameof(GetOpportunityById),
                    new { id = opportunity.OpportunityId },
                    new ApiResponse<Opportunitydto>(opportunityDto, "Opportunity created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating opportunity");
                throw;
            }
        }

        /// <summary>
        /// Update an existing opportunity
        /// </summary>
        /// <param name="id">Opportunity ID</param>
        /// <param name="updateOpportunitydto">Opportunity update data</param>
        /// <returns>Updated opportunity details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Opportunitydto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Opportunitydto>>> UpdateOpportunity(
            Guid id,
            [FromBody] UpdateOpportunitydto updateOpportunitydto)
        {
            try
            {
                _logger.LogInformation("Updating opportunity {OpportunityId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateOpportunitydto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for opportunity update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing customer
                var existingOpportunity = await _unitOfWork.Opportunity.GetByIdAsync(id);
                if (existingOpportunity == null)
                {
                    _logger.LogWarning("Opportunity {OpportunityId} not found", id);
                    return NotFound(new ApiResponse<object>($"Opportunity with ID {id} not found"));
                }

                // Update only provided fields
                

                if (!string.IsNullOrWhiteSpace(updateOpportunitydto.OpportunityName))
                    existingOpportunity.OpportunityName = updateOpportunitydto.OpportunityName;

               
                
                if (updateOpportunitydto.AssignedUserId.HasValue)
                    existingOpportunity.AssignedUserId = updateOpportunitydto.AssignedUserId;

               
                existingOpportunity.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Opportunity.Update(existingOpportunity);
                await _unitOfWork.SaveChangesAsync();

                var opportunityDto = _mapper.Map<Opportunitydto>(existingOpportunity);

                _logger.LogInformation("Successfully updated opportunity {opportunityId}", id);

                return Ok(new ApiResponse<Opportunitydto>(opportunityDto, "Opportunity updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating opportunity {OpportunityId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a opportunity (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">OPportunity ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteOpportunity(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting opportunity {OpportunityId}", id);

                var opportunity = await _unitOfWork.Opportunity.GetByIdAsync(id);
                if (opportunity == null)
                {
                    _logger.LogWarning("Opportunity {OpportunityId} not found", id);
                    return NotFound(new ApiResponse<object>($"Opportunity with ID {id} not found"));
                }

                // Soft delete
               
                opportunity.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Opportunity.Update(opportunity);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted opportunity {OpportunityId}", id);

                return Ok(new ApiResponse<object>(null, "Opportunity deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting opportunity {OPportunityId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search opportunity by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching opportunity</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<Opportunitydto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Opportunitydto>>>> SearchOpportunity(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching opportunity for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var opportunity = await _unitOfWork.Opportunity.SearchOpportunityAsync(tenantId, searchTerm);
                var opportunityDtos = _mapper.Map<List<Opportunitydto>>(opportunity);

                _logger.LogInformation("Found {Count} opportunity matching search term", opportunityDtos.Count);

                return Ok(new ApiResponse<List<Opportunitydto>>(opportunityDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching opportunity");
                throw;
            }
        }

        /// <summary>
        /// Get active opportunities for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active opportunity</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<Opportunitydto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Opportunitydto>>>> GetActiveOpportunity([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active opportunity for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var opportunity = await _unitOfWork.Opportunity.GetActiveOpportunityAsync(tenantId);
                var opportunityDtos = _mapper.Map<List<Opportunitydto>>(opportunity);

                _logger.LogInformation("Successfully retrieved {Count} active opportunity", opportunityDtos.Count);

                return Ok(new ApiResponse<List<Opportunitydto>>(opportunityDtos, "Active opportunity retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active opportunity for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}




    