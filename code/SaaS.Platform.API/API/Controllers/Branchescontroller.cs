using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Branches;
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
    public class BranchesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BranchesController> _logger;
        private readonly IValidator<CreateBranchesdto> _createValidator;
        private readonly IValidator<UpdateBranchesdto> _updateValidator;

        public BranchesController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<BranchesController> logger,
            IValidator<CreateBranchesdto> createValidator,
            IValidator<UpdateBranchesdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all branches with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of branches</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<Branchesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<Branchesdto>>> GetBranches(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching branches for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (branches, totalCount) = await _unitOfWork.Branches.GetBranchesPagedAsync(
                    tenantId, searchTerm,  pageNumber, pageSize);

                var branchDtos = _mapper.Map<List<Branchesdto>>(branches);

                var response = new PagedResponse<Branchesdto>(branchDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} branches out of {TotalCount} for tenant {TenantId}",
                    branchDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching branches for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get branch by ID
        /// </summary>
        /// <param name="id">Branch ID</param>
        /// <returns>Branch details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Branchesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Branchesdto>>> GetBranchesById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching branch with ID: {BranchId}", id);

                var branch = await _unitOfWork.Branches.GetByIdAsync(id);

                if (branch == null)
                {
                    _logger.LogWarning("Branch with ID {BranchId} not found", id);
                    return NotFound(new ApiResponse<object>($"Branch with ID {id} not found"));
                }

                var branchDto = _mapper.Map<Branchesdto>(branch);

                _logger.LogInformation("Successfully retrieved branch {BranchId}", id);

                return Ok(new ApiResponse<Branchesdto>(branchDto, "Branches retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching branch {BranchId}", id);
                throw;
            }
        }

        /// <summary>
        /// Get customer by branch code
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="branchCode">Branch code</param>
        /// <returns>Branch details</returns>
        [HttpGet("by-code/{branchCode}")]
        [ProducesResponseType(typeof(ApiResponse<Branchesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Branchesdto>>> GetBranchesByCode(
            [FromQuery] Guid tenantId,
            string branchCode)
        {
            try
            {
                _logger.LogInformation("Fetching branch with code {BranchCode} for tenant {TenantId}",
                    branchCode, tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var branch = await _unitOfWork.Branches.GetByBranchesCodeAsync(tenantId, branchCode);

                if (branch == null)
                {
                    _logger.LogWarning("Branch with code {BranchCode} not found for tenant {TenantId}",
                        branchCode, tenantId);
                    return NotFound(new ApiResponse<object>($"Branch with code {branchCode} not found"));
                }

                var branchDto = _mapper.Map<Branchesdto>(branch);

                _logger.LogInformation("Successfully retrieved branch with code {BranchCode}", branchCode);

                return Ok(new ApiResponse<Branchesdto>(branchDto, "Branch retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching branch with code {BranchCode}", branchCode);
                throw;
            }
        }

        /// <summary>
        /// Create a new branch
        /// </summary>
        /// <param name="createBranchesdto">Branch creation data</param>
        /// <returns>Created branch details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Branchesdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Branchesdto>>> CreateBranches([FromBody] CreateBranchesdto createBranchesdto)
        {
            try
            {
                _logger.LogInformation("Creating new branch with code {BranchCode} for tenant {TenantId}",
                    createBranchesdto.BranchCode, createBranchesdto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createBranchesdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Check if branch code is unique
                var isUnique = await _unitOfWork.Branches.IsBranchesCodeUniqueAsync(
                    createBranchesdto.TenantId, createBranchesdto.BranchCode);

                if (!isUnique)
                {
                    _logger.LogWarning("Branch code {Branchcode} already exists for tenant {TenantId}",
                        createBranchesdto.BranchCode, createBranchesdto.TenantId);
                    return BadRequest(new ApiResponse<object>("Branch code already exists"));
                }

                // Map DTO to entity
                var branch = _mapper.Map<Branches>(createBranchesdto);
                branch.BranchId = Guid.NewGuid();
                branch.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.Branches.AddAsync(branch);
                await _unitOfWork.SaveChangesAsync();

                var branchDto = _mapper.Map<Branchesdto>(branch);

                _logger.LogInformation("Successfully created branch {BranchId} with code {BranchCode}",
                    branch.BranchId, branch.BranchCode);

                return CreatedAtAction(
                    nameof(GetBranchesById),
                    new { id = branch.BranchId },
                    new ApiResponse<Branchesdto>(branchDto, "Branch created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating branch");
                throw;
            }
        }

        /// <summary>
        /// Update an existing branch
        /// </summary>
        /// <param name="id">Branch ID</param>
        /// <param name="updateBranchesdto">Branch update data</param>
        /// <returns>Updated branch details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Branchesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Branchesdto>>> UpdateBranches(
            Guid id,
            [FromBody] UpdateBranchesdto updateBranchesdto)
        {
            try
            {
                _logger.LogInformation("Updating branch {BranchId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateBranchesdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing branch
                var existingBranch = await _unitOfWork.Branches.GetByIdAsync(id);
                if (existingBranch == null)
                {
                    _logger.LogWarning("Branch {BranchId} not found", id);
                    return NotFound(new ApiResponse<object>($"Branch with ID {id} not found"));
                }

                // Update only provided fields

                if (!string.IsNullOrWhiteSpace(updateBranchesdto.BranchName))
                    existingBranch.BranchName = updateBranchesdto.BranchName;

                if (!string.IsNullOrWhiteSpace(updateBranchesdto.Email))
                    existingBranch.Email = updateBranchesdto.Email;

                if (!string.IsNullOrWhiteSpace(updateBranchesdto.Phone))
                    existingBranch.Phone = updateBranchesdto.Phone;

                if (!string.IsNullOrWhiteSpace(updateBranchesdto.Address))
                    existingBranch.Address = updateBranchesdto.Address;

                if (!string.IsNullOrWhiteSpace(updateBranchesdto.City))
                    existingBranch.City = updateBranchesdto.City;

                if (!string.IsNullOrWhiteSpace(updateBranchesdto.State))
                    existingBranch.State = updateBranchesdto.State;

                if (!string.IsNullOrWhiteSpace(updateBranchesdto.Country))
                    existingBranch.Country = updateBranchesdto.Country;

                if (!string.IsNullOrWhiteSpace(updateBranchesdto.PostalCode))
                    existingBranch.PostalCode = updateBranchesdto.PostalCode;

                if (updateBranchesdto.IsActive.HasValue)
                    existingBranch.IsActive = updateBranchesdto.IsActive.Value;

                existingBranch.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Branches.Update(existingBranch);
                await _unitOfWork.SaveChangesAsync();

                var branchDto = _mapper.Map<Branchesdto>(existingBranch);

                _logger.LogInformation("Successfully updated branch {BranchId}", id);

                return Ok(new ApiResponse<Branchesdto>(branchDto, "Branch updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating branch {BranchId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a branch (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Branch ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteBranches(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting branch {BranchId}", id);

                var branch = await _unitOfWork.Branches.GetByIdAsync(id);
                if (branch == null)
                {
                    _logger.LogWarning("Branch {BranchId} not found", id);
                    return NotFound(new ApiResponse<object>($"Branch with ID {id} not found"));
                }

                // Soft delete
                branch.IsActive = false;
                branch.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Branches.Update(branch);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted branch {BranchId}", id);

                return Ok(new ApiResponse<object>(null, "Branch deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting branch {BranchId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search branches by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching branches</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<Branchesdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Branchesdto>>>> SearchBranches(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching branches for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var branch = await _unitOfWork.Branches.SearchBranchesAsync(tenantId, searchTerm);
                var branchDtos = _mapper.Map<List<Branchesdto>>(branch);

                _logger.LogInformation("Found {Count} branches matching search term", branchDtos.Count);

                return Ok(new ApiResponse<List<Branchesdto>>(branchDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching branches");
                throw;
            }
        }

        /// <summary>
        /// Get active branches for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active branches</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<Branchesdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Branchesdto>>>> GetActiveBranches([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active branches for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var branch = await _unitOfWork.Branches.GetActiveBranchesAsync(tenantId);
                var branchDtos = _mapper.Map<List<Branchesdto>>(branch);

                _logger.LogInformation("Successfully retrieved {Count} active branches", branchDtos.Count);

                return Ok(new ApiResponse<List<Branchesdto>>(branchDtos, "Active branch retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active branches for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}
