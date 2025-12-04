using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Permissions;
using SaaS.Platform.API.Application.DTOs.SubscriptionsPlan;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// Permissions management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class PermissionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PermissionsController> _logger;
        private readonly IValidator<CreatePermissionsdto> _createValidator;
        private readonly IValidator<UpdatePermissionsdto> _updateValidator;

        public PermissionsController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<PermissionsController> logger,
            IValidator<CreatePermissionsdto> createValidator,
            IValidator<UpdatePermissionsdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }


        /// <summary>
        /// Get permissions by ID
        /// </summary>
        /// <param name="id">Permission ID</param>
        /// <returns>Permissions details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Permissionsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Permissionsdto>>> GetPermissionsById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching permissions with ID: {PermissionId}", id);

                var permission = await _unitOfWork.Permissions.GetByIdAsync(id);

                if (permission == null)
                {
                    _logger.LogWarning("Permissions with ID {PermissionId} not found", id);
                    return NotFound(new ApiResponse<object>($"Permissions with ID {id} not found"));
                }

                var permissionsdto = _mapper.Map<Permissionsdto>(permission);

                _logger.LogInformation("Successfully retrieved permission {PermissionId}", id);

                return Ok(new ApiResponse<Permissionsdto>(permissionsdto, "Permissions retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching permissions {PermissionId}", id);
                throw;
            }
        }

        /// <summary>
        /// Update an existing permission
        /// </summary>
        /// <param name="id">Permission ID</param>
        /// <param name="updatePermissiondto">Permission update data</param>
        /// <returns>Updated permissions details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Permissionsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Permissionsdto>>> UpdatePermission(
            Guid id,
            [FromBody] UpdatePermissionsdto updatePermissionsdto)
        {
            try
            {
                _logger.LogInformation("Updating permissions {PermissionId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updatePermissionsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for permissions update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing permissions
                var existingPermission = await _unitOfWork.Permissions.GetByIdAsync(id);
                if (existingPermission == null)
                {
                    _logger.LogWarning("Permission {PermissionId} not found", id);
                    return NotFound(new ApiResponse<object>($"Permission with ID {id} not found"));
                }

                // Update only provided fields


                if (updatePermissionsdto.IsActive.HasValue)
                    existingPermission.IsActive = updatePermissionsdto.IsActive.Value;

               

                _unitOfWork.Permissions.Update(existingPermission);
                await _unitOfWork.SaveChangesAsync();

                var permissionsdto = _mapper.Map<Permissionsdto>(existingPermission);

                _logger.LogInformation("Successfully updated permission {PermissionId}", id);

                return Ok(new ApiResponse<Permissionsdto>(permissionsdto, "Permission updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating permission {PermissionId}", id);
                throw;
            }
        }


    }
}
