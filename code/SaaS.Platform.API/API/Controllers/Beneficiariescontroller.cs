using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// Beneficiary management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class BeneficiariesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BeneficiariesController> _logger;
        private readonly IValidator<CreateBeneficiariesdto> _createValidator;
        private readonly IValidator<UpdateBeneficiariesdto> _updateValidator;

        public BeneficiariesController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<BeneficiariesController> logger,
            IValidator<CreateBeneficiariesdto> createValidator,
            IValidator<UpdateBeneficiariesdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all beneficiary with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of beneficiary</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<Beneficiariesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<Beneficiariesdto>>> GetBeneficiaries(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching beneficiary for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (beneficiaries, totalCount) = await _unitOfWork.Beneficiaries.GetBeneficiariesPagedAsync(
                    tenantId, searchTerm, pageNumber, pageSize);

                var beneficiariesDtos = _mapper.Map<List<Beneficiariesdto>>(beneficiaries);

                var response = new PagedResponse<Beneficiariesdto>(beneficiariesDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} beneficiry out of {TotalCount} for tenant {TenantId}",
                    beneficiariesDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching beneficiary for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get beneficiary by ID
        /// </summary>
        /// <param name="id">Beneficiary ID</param>
        /// <returns>Beneficiary details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Beneficiariesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Beneficiariesdto>>> GetBeneficiariesById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching beneficiry with ID: {BeneficiaryId}", id);

                var beneficiaries = await _unitOfWork.Beneficiaries.GetByIdAsync(id);

                if (beneficiaries == null)
                {
                    _logger.LogWarning("Beneficiary with ID {BeneficiaryId} not found", id);
                    return NotFound(new ApiResponse<object>($"Beneficiary with ID {id} not found"));
                }

                var beneficiariesDto = _mapper.Map<Beneficiariesdto>(beneficiaries);

                _logger.LogInformation("Successfully retrieved beneficry {BeneficiaryId}", id);

                return Ok(new ApiResponse<Beneficiariesdto>(beneficiariesDto, "Beneficiry retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching beneficiary {BeneficiaryId}", id);
                throw;
            }
        }



        /// <summary>
        /// Create a new beneficiary
        /// </summary>
        /// <param name="createBeneficiariesdto">Beneficiaries creation data</param>
        /// <returns>Created beneficiary details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Beneficiariesdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Beneficiariesdto>>> CreateBeneficiaries([FromBody] CreateBeneficiariesdto createBeneficiariesdto)
        {
            try
            {
                _logger.LogInformation("Creating new beneficiary  for tenant {TenantId}",
                     createBeneficiariesdto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createBeneficiariesdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for beneficiary creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }


                // Map DTO to entity
                var beneficiaries = _mapper.Map<Beneficiaries>(createBeneficiariesdto);
                beneficiaries.BeneficiaryId = Guid.NewGuid();
                beneficiaries.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.Beneficiaries.AddAsync(beneficiaries);
                await _unitOfWork.SaveChangesAsync();

                var beneficiariesDto = _mapper.Map<Beneficiariesdto>(beneficiaries);

                _logger.LogInformation("Successfully created beneficiary {BeneficiaryId} ",
                    beneficiaries.BeneficiaryId);

                return CreatedAtAction(
                    nameof(GetBeneficiariesById),
                    new { id = beneficiaries.BeneficiaryId },
                    new ApiResponse<Beneficiariesdto>(beneficiariesDto, "Beneficiary created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating beneficiary");
                throw;
            }
        }

        /// <summary>
        /// Update an existing beneficiary
        /// </summary>
        /// <param name="id">Beneficiary ID</param>
        /// <param name="updateBeneficiariesdto">Beneficiary update data</param>
        /// <returns>Updated beneficiary details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Beneficiariesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Beneficiariesdto>>> UpdateBeneficiaries(
            Guid id,
            [FromBody] UpdateBeneficiariesdto updateBeneficiariesdto)
        {
            try
            {
                _logger.LogInformation("Updating beneficiary {BeneficiaryId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateBeneficiariesdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing beneficiary
                var existingBeneficiary = await _unitOfWork.Beneficiaries.GetByIdAsync(id);
                if (existingBeneficiary == null)
                {
                    _logger.LogWarning("Beneficiary {BeneficiaryId} not found", id);
                    return NotFound(new ApiResponse<object>($"Beneficiary with ID {id} not found"));
                }

                // Update only provided fields

                if (!string.IsNullOrWhiteSpace(updateBeneficiariesdto.BeneficiaryName))
                    existingBeneficiary.BeneficiaryName = updateBeneficiariesdto.BeneficiaryName;


                if (updateBeneficiariesdto.IsActive.HasValue)
                    existingBeneficiary.IsActive = updateBeneficiariesdto.IsActive.Value;

                existingBeneficiary.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Beneficiaries.Update(existingBeneficiary);
                await _unitOfWork.SaveChangesAsync();

                var beneficiariesDto = _mapper.Map<Beneficiariesdto>(existingBeneficiary);

                _logger.LogInformation("Successfully updated beneficiary {BeneficiaryId}", id);

                return Ok(new ApiResponse<Beneficiariesdto>(beneficiariesDto, "Beneficiary updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating beneficiary {BeneficiaryId}", id);
                throw;
            }
        }
        /// <summary>
        /// Delete a beneficiary (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Beneficiary ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteBeneficiary(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting beneficiary {BeneficiaryId}", id);

                var beneficiary = await _unitOfWork.Beneficiaries.GetByIdAsync(id);
                if (beneficiary == null)
                {
                    _logger.LogWarning("Beneficiary {BeneficiaryId} not found", id);
                    return NotFound(new ApiResponse<object>($"Beneficiary with ID {id} not found"));
                }

                // Soft delete
                beneficiary.IsActive = false;
                beneficiary.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Beneficiaries.Update(beneficiary);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted beneficiary {BeneficiaryId}", id);

                return Ok(new ApiResponse<object>(null, "Beneficiary deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting beneficiary {BeneficiaryId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search beneficiary by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching beneficiary</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<Beneficiariesdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Beneficiariesdto>>>> SearchBeneficiary(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching beneficiary for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var beneficiary = await _unitOfWork.Beneficiaries.SearchBeneficiariesAsync(tenantId, searchTerm);
                var beneficiaryDtos = _mapper.Map<List<Beneficiariesdto>>(beneficiary);

                _logger.LogInformation("Found {Count} beneficiary matching search term", beneficiaryDtos.Count);

                return Ok(new ApiResponse<List<Beneficiariesdto>>(beneficiaryDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching customers");
                throw;
            }
        }

        /// <summary>
        /// Get active beneficiary for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active beneficiaries</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<Beneficiariesdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Beneficiariesdto>>>> GetActiveBeneficiaries([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active beneficiarys for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var beneficiary = await _unitOfWork.Beneficiaries.GetActiveBeneficiariesAsync(tenantId);
                var beneficiaryDtos = _mapper.Map<List<Beneficiariesdto>>(beneficiary);

                _logger.LogInformation("Successfully retrieved {Count} active beneficiaries", beneficiaryDtos.Count);

                return Ok(new ApiResponse<List<Beneficiariesdto>>(beneficiaryDtos, "Active beneficiaries retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active beneficiary for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}
    