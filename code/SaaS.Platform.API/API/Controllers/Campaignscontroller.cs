using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Campaigns;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// Campaign management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class CampaignsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CampaignsController> _logger;
        private readonly IValidator<CreateCampaigndto> _createValidator;
        private readonly IValidator<UpdateCampaigndto> _updateValidator;

        public CampaignsController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CampaignsController> logger,
            IValidator<CreateCampaigndto> createValidator,
            IValidator<UpdateCampaigndto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all campigns with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of campign</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<Campaigndto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<Campaigndto>>> GetCampaigns(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching campaigns for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (campaigns, totalCount) = await _unitOfWork.Campaigns.GetCampaignsPagedAsync(
                    tenantId, searchTerm, pageNumber, pageSize);

                var campaignDtos = _mapper.Map<List<Campaigndto>>(campaigns);

                var response = new PagedResponse<Campaigndto>(campaignDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} campaign out of {TotalCount} for tenant {TenantId}",
                    campaignDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching campaigns for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get campaign by ID
        /// </summary>
        /// <param name="id">Campaign ID</param>
        /// <returns>Campaign details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Campaigndto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Campaigndto>>> GetCampaignById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching campaign with ID: {CampaignId}", id);

                var campaign = await _unitOfWork.Campaigns.GetByIdAsync(id);

                if (campaign == null)
                {
                    _logger.LogWarning("Campaign with ID {CampaignId} not found", id);
                    return NotFound(new ApiResponse<object>($"Campaign with ID {id} not found"));
                }

                var campaignDto = _mapper.Map<Campaigndto>(campaign);

                _logger.LogInformation("Successfully retrieved campaign {CampaignId}", id);

                return Ok(new ApiResponse<Campaigndto>(campaignDto, "Campaign retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching campaign {CampaignId}", id);
                throw;
            }
        }

       
        /// <summary>
        /// Create a new campaign
        /// </summary>
        /// <param name="createCampaigndto">Campaign creation data</param>
        /// <returns>Created campaign details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Campaigndto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Campaigndto>>> CreateCampaign([FromBody] CreateCampaigndto createCampaigndto)
        {
            try
            {
                _logger.LogInformation("Creating new campaign  for tenant {TenantId}",
                     createCampaigndto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createCampaigndto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

               
                // Map DTO to entity
                var campaign = _mapper.Map<Campaigns>(createCampaigndto);
                campaign.CampaignId = Guid.NewGuid();
                campaign.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.Campaigns.AddAsync(campaign);
                await _unitOfWork.SaveChangesAsync();

                var campaignDto = _mapper.Map<Campaigndto>(campaign);

                _logger.LogInformation("Successfully created campaign {CampaignId}",
                    campaign.CampaignId);

                return CreatedAtAction(
                    nameof(GetCampaignById),
                    new { id = campaign.CampaignId },
                    new ApiResponse<Campaigndto>(campaignDto, "Campaigns created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating campaign");
                throw;
            }
        }

        /// <summary>
        /// Update an existing campaign
        /// </summary>
        /// <param name="id">Campaign ID</param>
        /// <param name="updateCustomerDto">Campaign update data</param>
        /// <returns>Updated campaign details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Campaigndto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Campaigndto>>> UpdateCampaign(
            Guid id,
            [FromBody] UpdateCampaigndto updateCampaigndto)
        {
            try
            {
                _logger.LogInformation("Updating campaign {CampaignId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateCampaigndto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing campaign
                var existingCampaign = await _unitOfWork.Campaigns.GetByIdAsync(id);
                if (existingCampaign == null)
                {
                    _logger.LogWarning("Campaign {CampaignId} not found", id);
                    return NotFound(new ApiResponse<object>($"Campaign with ID {id} not found"));
                }

                // Update only provided fields

                if (!string.IsNullOrWhiteSpace(updateCampaigndto.CampaignName))
                    existingCampaign.CampaignName = updateCampaigndto.CampaignName;


                existingCampaign.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Campaigns.Update(existingCampaign);
                await _unitOfWork.SaveChangesAsync();

                var campaignDto = _mapper.Map<Campaigndto>(existingCampaign);

                _logger.LogInformation("Successfully updated campaign {CampaignId}", id);

                return Ok(new ApiResponse<Campaigndto>(campaignDto, "Campaign updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating campaign {CampaignId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a campaign (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Campaign ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteCampaign(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting campaign {CampaignId}", id);

                var campaign = await _unitOfWork.Campaigns.GetByIdAsync(id);
                if (campaign == null)
                {
                    _logger.LogWarning("Campaign {CampaignId} not found", id);
                    return NotFound(new ApiResponse<object>($"Campaigns with ID {id} not found"));
                }

                // Soft delete
                campaign.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Campaigns.Update(campaign);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted campaign {CampaignId}", id);

                return Ok(new ApiResponse<object>(null, "Campaign deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting campaign {CampaignId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search campaigns by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching campaigns</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<Campaigndto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Campaigndto>>>> SearchCampaignd(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching campaigns for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var campaigns = await _unitOfWork.Campaigns.SearchCampaignsAsync(tenantId, searchTerm);
                var campaignDtos = _mapper.Map<List<Campaigndto>>(campaigns);

                _logger.LogInformation("Found {Count} campaigns matching search term", campaignDtos.Count);

                return Ok(new ApiResponse<List<Campaigndto>>(campaignDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching campaigns");
                throw;
            }
        }

        /// <summary>
        /// Get active campaigns for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active campaigns</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<Campaigndto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Campaigndto>>>> GetActiveCampaigns([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active campaigns for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var campaigns = await _unitOfWork.Campaigns.GetActiveCampaignsAsync(tenantId);
                var campaignDtos = _mapper.Map<List<Campaigndto>>(campaigns);

                _logger.LogInformation("Successfully retrieved {Count} active campaigns", campaignDtos.Count);

                return Ok(new ApiResponse<List<Campaigndto>>(campaignDtos, "Active campaigns retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active campaigns for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}