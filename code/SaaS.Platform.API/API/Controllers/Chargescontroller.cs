using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Charges;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// Charges management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class ChargesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ChargesController> _logger;
        private readonly IValidator<CreateChargesdto> _createValidator;
        private readonly IValidator<UpdateChargesdto> _updateValidator;

        public ChargesController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ChargesController> logger,
            IValidator<CreateChargesdto> createValidator,
            IValidator<UpdateChargesdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all charges with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of charges</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<Chargesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<Chargesdto>>> GetCharges(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching charges for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (charges, totalCount) = await _unitOfWork.Charges.GetChargesPagedAsync(
                    tenantId, searchTerm, pageNumber, pageSize);

                var chargeDtos = _mapper.Map<List<Chargesdto>>(charges);

                var response = new PagedResponse<Chargesdto>(chargeDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} charges out of {TotalCount} for tenant {TenantId}",
                    chargeDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching charges for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get charge by ID
        /// </summary>
        /// <param name="id">Charge ID</param>
        /// <returns>Charges details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Chargesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Chargesdto>>> GetChargesById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching charges with ID: {ChargeId}", id);

                var charges = await _unitOfWork.Charges.GetByIdAsync(id);

                if (charges == null)
                {
                    _logger.LogWarning("Charge with ID {ChargeId} not found", id);
                    return NotFound(new ApiResponse<object>($"Charge with ID {id} not found"));
                }

                var chargeDto = _mapper.Map<Chargesdto>(charges);

                _logger.LogInformation("Successfully retrieved charges {ChargeId}", id);

                return Ok(new ApiResponse<Chargesdto>(chargeDto, "Charges retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching charges {ChargeId}", id);
                throw;
            }
        }

        /// <summary>
        /// Get charges by charge code
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="chargeCode">Charge code</param>
        /// <returns>Charges details</returns>
        [HttpGet("by-code/{chargeCode}")]
        [ProducesResponseType(typeof(ApiResponse<Chargesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Chargesdto>>> GetChargesByCode(
            [FromQuery] Guid tenantId,
            string chargeCode)
        {
            try
            {
                _logger.LogInformation("Fetching charge with code {ChargeCode} for tenant {TenantId}",
                    chargeCode, tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var charges = await _unitOfWork.Charges.GetByChargesCodeAsync(tenantId, chargeCode);

                if (charges == null)
                {
                    _logger.LogWarning("Charge with code {ChargeCode} not found for tenant {TenantId}",
                      chargeCode, tenantId);
                    return NotFound(new ApiResponse<object>($"Charge with code {chargeCode} not found"));
                }

                var chargeDto = _mapper.Map<Chargesdto>(charges);

                _logger.LogInformation("Successfully retrieved charge with code {ChargeCode}", chargeCode);

                return Ok(new ApiResponse<Chargesdto>(chargeDto, "Charges retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching charges with code {ChargeCode}", chargeCode);
                throw;
            }
        }

        /// <summary>
        /// Create a new charge
        /// </summary>
        /// <param name="createChargesdto">Charges creation data</param>
        /// <returns>Created charges details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Chargesdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Chargesdto>>> CreateCharges([FromBody] CreateChargesdto createChargesdto)
        {
            try
            {
                _logger.LogInformation("Creating new charge with code {ChargeCode} for tenant {TenantId}",
                    createChargesdto.ChargeCode, createChargesdto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createChargesdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Check if charges code is unique
                var isUnique = await _unitOfWork.Charges.IsChargesCodeUniqueAsync(
                    createChargesdto.TenantId, createChargesdto.ChargeCode);

                if (!isUnique)
                {
                    _logger.LogWarning("Charge code {ChargeCode} already exists for tenant {TenantId}",
                        createChargesdto.ChargeCode, createChargesdto.TenantId);
                    return BadRequest(new ApiResponse<object>("Charge code already exists"));
                }

                // Map DTO to entity
                var charge = _mapper.Map<Charges>(createChargesdto);
                charge.ChargeId = Guid.NewGuid();
                charge.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.Charges.AddAsync(charge);
                await _unitOfWork.SaveChangesAsync();

                var chargeDto = _mapper.Map<Chargesdto>(charge);

                _logger.LogInformation("Successfully created charges {ChargeId} with code {ChargeCode}",
                    charge.ChargeId, charge.ChargeCode);

                return CreatedAtAction(
                    nameof(GetChargesById),
                    new { id = charge.ChargeId },
                    new ApiResponse<Chargesdto>(chargeDto, "Charges created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating charges");
                throw;
            }
        }

        /// <summary>
        /// Update an existing charges
        /// </summary>
        /// <param name="id">Charge ID</param>
        /// <param name="updateChargesdto">Charges update data</param>
        /// <returns>Updated charge details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Chargesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Chargesdto>>> UpdateCahrges(
            Guid id,
            [FromBody] UpdateChargesdto updateChargesdto)
        {
            try
            {
                _logger.LogInformation("Updating charge {ChargeId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateChargesdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing charge
                var existingCharge = await _unitOfWork.Charges.GetByIdAsync(id);
                if (existingCharge == null)
                {
                    _logger.LogWarning("Charges {ChargeId} not found", id);
                    return NotFound(new ApiResponse<object>($"Charge with ID {id} not found"));
                }

                // Update only provided fields
                
                if (!string.IsNullOrWhiteSpace(updateChargesdto.ChargeName))
                    existingCharge.ChargeName = updateChargesdto.ChargeName;

                if (updateChargesdto.IsActive.HasValue)
                  existingCharge.IsActive = updateChargesdto.IsActive.Value;

                _unitOfWork.Charges.Update(existingCharge);
                await _unitOfWork.SaveChangesAsync();

                var chargeDto = _mapper.Map<Chargesdto>(existingCharge);

                _logger.LogInformation("Successfully updated charge {ChargeId}", id);

                return Ok(new ApiResponse<Chargesdto>(chargeDto, "Charge updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating charge {ChargeId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a charge (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Charge ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteCharges(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting charge {ChargeId}", id);

                var charge = await _unitOfWork.Charges.GetByIdAsync(id);
                if (charge == null)
                {
                    _logger.LogWarning("Charges {ChargeId} not found", id);
                    return NotFound(new ApiResponse<object>($"Charge with ID {id} not found"));
                }

                // Soft delete
                charge.IsActive = false;
                
                _unitOfWork.Charges.Update(charge);
                 await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted charge {ChargeId}", id);

                return Ok(new ApiResponse<object>(null, "Charge deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting charge {ChargeId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search charges by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching charges</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<Chargesdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Chargesdto>>>> SearchCharges(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching chargess for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var charges = await _unitOfWork.Charges.SearchChargesAsync(tenantId, searchTerm);
                var chargeDtos = _mapper.Map<List<Chargesdto>>(charges);

                _logger.LogInformation("Found {Count} charges matching search term", chargeDtos.Count);

                return Ok(new ApiResponse<List<Chargesdto>>(chargeDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching charges");
                throw;
            }
        }

        /// <summary>
        /// Get active charges for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active charges</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<Chargesdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Chargesdto>>>> GetActiveCharges([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active charges for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var charges = await _unitOfWork.Charges.GetActiveChargesAsync(tenantId);
                var chargeDtos = _mapper.Map<List<Chargesdto>>(charges);

                _logger.LogInformation("Successfully retrieved {Count} active chargess", chargeDtos.Count);

                return Ok(new ApiResponse<List<Chargesdto>>(chargeDtos, "Active charges retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active charges for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}