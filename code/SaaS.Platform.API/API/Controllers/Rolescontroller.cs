using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Customer;
using SaaS.Platform.API.Application.DTOs.Roles;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Domain.Entities.Roles.cs;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// Roles management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class RolesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RolesController> _logger;
        private readonly IValidator<CreateRolesdto> _createValidator;
        private readonly IValidator<UpdateRolesdto> _updateValidator;

        public RolesController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<RolesController> logger,
            IValidator<CreateRolesdto> createValidator,
            IValidator<UpdateRolesdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all roles with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of roles</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<Rolesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<Rolesdto>>> GetRoles(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching roles for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (roles, totalCount) = await _unitOfWork.Roles.GetRolesPagedAsync(
                    tenantId, searchTerm, pageNumber, pageSize);

                var rolesDtos = _mapper.Map<List<Rolesdto>>(roles);

                var response = new PagedResponse<Rolesdto>(rolesDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} roles out of {TotalCount} for tenant {TenantId}",
                    rolesDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching roles for tenant {TenantId}", tenantId);
                throw;
            }
        }

            /// </summary>
            /// <param name="id">Role ID</param>
            /// <returns>Roles details</returns>
            [HttpGet("{id}")]
            [ProducesResponseType(typeof(ApiResponse<Rolesdto>), StatusCodes.Status200OK)]
            [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]

            public async Task<ActionResult<ApiResponse<Rolesdto>>> GetRolesById(Guid id)
            {
                try
                {
                    _logger.LogInformation("Fetching roles with ID: {RolesId}", id);

                    var roles = await _unitOfWork.Roles.GetByIdAsync(id);

                    if (roles == null)
                    {
                        _logger.LogWarning("Roles with ID {RoleId} not found", id);
                        return NotFound(new ApiResponse<object>($"Roles with ID {id} not found"));
                    }

                    var roleDto = _mapper.Map<Rolesdto>(roles);

                    _logger.LogInformation("Successfully retrieved roles {RoleId}", id);

                    return Ok(new ApiResponse<Rolesdto>(roleDto, "Roles retrieved successfully"));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while fetching roles {RolesId}", id);
                    throw;
                }
            }

        /// <summary>
        /// Create a new roles
        /// </summary>
        /// <param name="createRolesdto">Roles creation data</param>
        /// <returns>Created roles details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Rolesdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Rolesdto>>> CreateRoles([FromBody] CreateRolesdto createRoledto)
        {
            try
            {
                _logger.LogInformation("Creating new roles  for tenant {TenantId}",
                     createRoledto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createRoledto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for roles creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

               
                // Map DTO to entity
                var roles = _mapper.Map<Roles>(createRoledto);
                roles.RoleId = Guid.NewGuid();
                roles.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.Roles.AddAsync(roles);
                await _unitOfWork.SaveChangesAsync();

                var rolesDto = _mapper.Map<Rolesdto>(roles);

                _logger.LogInformation("Successfully created roles {RoleId} )",
                    roles.RoleId);

                return CreatedAtAction(
                    nameof(GetRolesById),
                    new { id = roles.RoleId },
                    new ApiResponse<Rolesdto>(rolesDto, "Roles created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating roles");
                throw;
            }
        }

        /// <summary>
        /// Update an existing roles
        /// </summary>
        /// <param name="id">Roles ID</param>
        /// <param name="updateRolesDto">Roles update data</param>
        /// <returns>Updated roles details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Rolesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Rolesdto>>> UpdateRoles(
            Guid id,
            [FromBody] UpdateRolesdto updateRolesdto)
        {
            try
            {
                _logger.LogInformation("Updating roles {RoleId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateRolesdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for roles update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing customer
                var existingRoles = await _unitOfWork.Roles.GetByIdAsync(id);
                if (existingRoles == null)
                {
                    _logger.LogWarning("Roles {RoleId} not found", id);
                    return NotFound(new ApiResponse<object>($"Roles with ID {id} not found"));
                }

                // Update only provided fields
                
                if (updateRolesdto.IsActive.HasValue)
                    existingRoles.IsActive = updateRolesdto.IsActive.Value;

                existingRoles.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Roles.Update(existingRoles);
                await _unitOfWork.SaveChangesAsync();

                var rolesDto = _mapper.Map<Rolesdto>(existingRoles);

                _logger.LogInformation("Successfully updated roles {RoleId}", id);

                return Ok(new ApiResponse<Rolesdto>(rolesDto, "Roles updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating roles {RoleId}", id);
                throw;
            }
        }
        /// <summary>
        /// Delete a roles (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Role ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteRoles(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting roles {RoleId}", id);

                var roles = await _unitOfWork.Roles.GetByIdAsync(id);
                if (roles == null)
                {
                    _logger.LogWarning("Roles {RoleId} not found", id);
                    return NotFound(new ApiResponse<object>($"Roles with ID {id} not found"));
                }

                // Soft delete
                roles.IsActive = false;
                roles.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Roles.Update(roles);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted roles {RoleId}", id);

                return Ok(new ApiResponse<object>(null, "Roles deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting customer {CustomerId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search roles by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching roles</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<Rolesdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Rolesdto>>>> SearchRoles(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching roles for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var roles = await _unitOfWork.Roles.SearchRolesAsync(tenantId, searchTerm);
                var rolesDtos = _mapper.Map<List<Rolesdto>>(roles);

                _logger.LogInformation("Found {Count} roles matching search term", rolesDtos.Count);

                return Ok(new ApiResponse<List<Rolesdto>>(rolesDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching roles");
                throw;
            }
        }

        /// <summary>
        /// Get active roles for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active roles</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<Rolesdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Rolesdto>>>> GetActiveRoles([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active roles for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var roles = await _unitOfWork.Roles.GetActiveRolesAsync(tenantId);
                var rolesDtos = _mapper.Map<List<Rolesdto>>(roles);

                _logger.LogInformation("Successfully retrieved {Count} active roles", rolesDtos.Count);

                return Ok(new ApiResponse<List<Rolesdto>>(rolesDtos, "Active roles retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active roles for tenant {TenantId}", tenantId);
                throw;
            }
        }



        }

}

