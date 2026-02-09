using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Charges;
using SaaS.Platform.API.Application.DTOs.LoanTypes;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// Loans management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class LoanTypesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<LoanTypesController> _logger;
        private readonly IValidator<CreateLoanTypesdto> _createValidator;
        private readonly IValidator<UpdateLoanTypesdto> _updateValidator;

        public LoanTypesController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<LoanTypesController> logger,
            IValidator<CreateLoanTypesdto> createValidator,
            IValidator<UpdateLoanTypesdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all loanTypes with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="status">loan status filter</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of loanTypes</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<LoanTypesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<LoanTypesdto>>> GetLoanTypes(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? status = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching loans for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (loanTypes, totalCount) = await _unitOfWork.LoanTypes.GetLoanTypesPagedAsync(
                    tenantId, searchTerm, pageNumber, pageSize);

                var loanTypeDtos = _mapper.Map<List<LoanTypesdto>>(loanTypes);

                var response = new PagedResponse<LoanTypesdto>(loanTypeDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} loanTypes out of {TotalCount} for tenant {TenantId}",
                    loanTypeDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching loanTypes for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get loanType by ID
        /// </summary>
        /// <param name="id">LoanType ID</param>
        /// <returns>Loan details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<LoanTypesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<LoanTypesdto>>> GetLoanTypesById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching loanTypes with ID: {LoanId}", id);

                var loanType = await _unitOfWork.LoanTypes.GetByIdAsync(id);

                if (loanType == null)
                {
                    _logger.LogWarning("LoanType with ID {LoanTypeId} not found", id);
                    return NotFound(new ApiResponse<object>($"LoanType with ID {id} not found"));
                }

                var loanTypeDto = _mapper.Map<LoanTypesdto>(loanType);

                _logger.LogInformation("Successfully retrieved loanType {LoanTypeId}", id);

                return Ok(new ApiResponse<LoanTypesdto>(loanTypeDto, "LoanType retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching LoanTypes {LoanTypeId}", id);
                throw;
            }
        }

        /// <summary>
        /// Create a new loanType
        /// </summary>
        /// <param name="createLoanTypesdto">LoanType creation data</param>
        /// <returns>Created loanType details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<LoanTypesdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<LoanTypesdto>>> CreateLoanTypes([FromBody] CreateLoanTypesdto createLoanTypesdto)
        {
            try
            {
                _logger.LogInformation("Creating new loanType  tenant {TenantId}",
                     createLoanTypesdto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createLoanTypesdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for loanType creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }



                // Map DTO to entity
                var loanType = _mapper.Map<LoanTypes>(createLoanTypesdto);
                loanType.LoanTypeId = Guid.NewGuid();
                loanType.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.LoanTypes.AddAsync(loanType);
                await _unitOfWork.SaveChangesAsync();

                var loanTypeDto = _mapper.Map<LoanTypesdto>(loanType);

                _logger.LogInformation("Successfully created loanType loanTypeId",
                    loanType.LoanTypeId);

                return CreatedAtAction(
                    nameof(GetLoanTypesById),
                    new { id = loanType.LoanTypeId },
                    new ApiResponse<LoanTypesdto>(loanTypeDto, "LoanType created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating loanType");
                throw;
            }
        }

        /// <summary>
        /// Update an existing loanType
        /// </summary>
        /// <param name="id">LoanType ID</param>
        /// <param name="updateLoanTypesdto">Loan update data</param>
        /// <returns>Updated loanTypes details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<LoanTypesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<LoanTypesdto>>> UpdateLoanTypes(
            Guid id,
            [FromBody] UpdateLoanTypesdto updateLoanTypesdto)
        {
            try
            {
                _logger.LogInformation("Updating loanType {LoanTypeId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateLoanTypesdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for laonType update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing loan
                var existingLoanType = await _unitOfWork.LoanTypes.GetByIdAsync(id);
                if (existingLoanType == null)
                {
                    _logger.LogWarning("LoanType {LoanTypeId} not found", id);
                    return NotFound(new ApiResponse<object>($"LoanType with ID {id} not found"));
                }

                // Update only provided fields

                if (!string.IsNullOrWhiteSpace(updateLoanTypesdto.LoanTypeName))
                    existingLoanType.LoanTypeName = updateLoanTypesdto.LoanTypeName;

                if (updateLoanTypesdto.IsActive.HasValue)
                    existingLoanType.IsActive = updateLoanTypesdto.IsActive.Value;



                _unitOfWork.LoanTypes.Update(existingLoanType);
                await _unitOfWork.SaveChangesAsync();

                var loanTypeDto = _mapper.Map<LoanTypesdto>(existingLoanType);

                _logger.LogInformation("Successfully updated loanType {LoanTypeId}", id);

                return Ok(new ApiResponse<LoanTypesdto>(loanTypeDto, "Loan updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating loanType {LoanTypeId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a loanType (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">LoanType ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteLoanTypes(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting LoanTypes {LoanTypeId}", id);

                var loanType = await _unitOfWork.Loans.GetByIdAsync(id);
                if (loanType == null)
                {
                    _logger.LogWarning("LoanTypes {LoanTypeId} not found", id);
                    return NotFound(new ApiResponse<object>($"LoanType with ID {id} not found"));
                }
               


                _unitOfWork.Loans.Update(loanType);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted loanType {LoanTypeId}", id);

                return Ok(new ApiResponse<object>(null, "LoanType deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting loanType {LoanTypeId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search loanTypes by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching loanTypes</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<LoanTypesdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<LoanTypesdto>>>> SearchLoanTypes(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching loanTypes for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var loanType = await _unitOfWork.LoanTypes.SearchLoanTypesAsync(tenantId, searchTerm);
                var loanTypeDtos = _mapper.Map<List<LoanTypesdto>>(loanType);

                _logger.LogInformation("Found {Count} loanTypes matching search term", loanTypeDtos.Count);

                return Ok(new ApiResponse<List<LoanTypesdto>>(loanTypeDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching loanTypes");
                throw;
            }
        }

        /// <summary>
        /// Get active loanTypes for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active laonTypes</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<LoanTypesdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<LoanTypesdto>>>> GetActiveLoanTypes([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active loanTypes for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var loanType = await _unitOfWork.LoanTypes.GetActiveLoanTypesAsync(tenantId);
                var loanTypeDtos = _mapper.Map<List<LoanTypesdto>>(loanType);

                _logger.LogInformation("Successfully retrieved {Count} active loanTypes", loanTypeDtos.Count);

                return Ok(new ApiResponse<List<LoanTypesdto>>(loanTypeDtos, "Active loanTypes retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active loanTypes for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}