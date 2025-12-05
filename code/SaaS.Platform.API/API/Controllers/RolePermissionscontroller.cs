using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.RolePermissions;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// RolePermissions management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class RolePermissionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RolePermissionsController> _logger;
        private readonly IValidator<CreateRolePermissionsdto> _createValidator;
        private readonly IValidator<UpdateRolePermissionsdto> _updateValidator;

        public RolePermissionsController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<RolePermissionsController> logger,
            IValidator<CreateRolePermissionsdto> createValidator,
            IValidator<UpdateRolePermissionsdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get RolePermissions by ID
        /// </summary>
        /// <param name="id">RolePermissions ID</param>
        /// <returns>RolePermissions details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<RolePermissionsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<RolePermissionsdto>>> GetByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching rolePermissions with ID: {RolePermissionsId}", id);

                var RolePermissionsdto = await _unitOfWork.RolePermissions.GetByIdAsync(id);

                if (RolePermissionsdto == null)
                {
                    _logger.LogWarning("RolePermissions with ID {RolePermissionsId} not found", id);
                    return NotFound(new ApiResponse<object>($"RolePermissions with ID {id} not found"));
                }

                var rolePermissionsDto = _mapper.Map<RolePermissionsdto>(RolePermissionsdto);

                _logger.LogInformation("Successfully retrieved rolePermissions {RolePermissionsId}", id);

                return Ok(new ApiResponse<RolePermissionsdto>(rolePermissionsDto, "RolePermissions retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching rolePermissions {RolePermissionsId}", id);
                throw;
            }
        }

        /// <summary>
        /// Update an existing Rolepermission
        /// </summary>
        /// <param name="id">Permission ID</param>
        /// <param name="updateRolePermissiondto">Permission update data</param>
        /// <returns>Updated permissions details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<RolePermissionsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<RolePermissionsdto>>> UpdateRolePermission(
            Guid id,
            [FromBody] UpdateRolePermissionsdto updateRolePermissionsdto)
        {
            try
            {
                _logger.LogInformation("Updating Rolepermissions {PermissionId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateRolePermissionsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for Rolepermissions update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing permissions
                var existingRolePermission = await _unitOfWork.Permissions.GetByIdAsync(id);
                if (existingRolePermission == null)
                {
                    _logger.LogWarning("RolePermission {PermissionId} not found", id);
                    return NotFound(new ApiResponse<object>($"RolePermission with ID {id} not found"));
                }

               

                _unitOfWork.Permissions.Update(existingRolePermission);
                await _unitOfWork.SaveChangesAsync();

                var rolepermissionsdto = _mapper.Map<RolePermissionsdto>(existingRolePermission);

                _logger.LogInformation("Successfully updated Rolepermission {PermissionId}", id);

                return Ok(new ApiResponse<RolePermissionsdto>(rolepermissionsdto, "Permission updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating Rolepermission {PermissionId}", id);
                throw;
            }
        }


    }
}

