using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.UserRoles;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// UserRoles management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class UsersRolesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;
        private readonly IValidator<CreateUserRolesdto> _createValidator;
        private readonly IValidator<UpdateUserRolesdto> _updateValidator;

        public UsersRolesController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UsersController> logger,
            IValidator<CreateUserRolesdto> createValidator,
            IValidator<UpdateUserRolesdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }


        /// <summary>
        /// Get userRoles by ID
        /// </summary>
        /// <param name="id">UserRoles ID</param>
        /// <returns>UserRoles details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserRolesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<UserRolesdto>>> GetUserRolesById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching userRoles with ID: {UserRolesId}", id);

                var userRoles = await _unitOfWork.UserRoles.GetByIdAsync(id);

                if (userRoles == null)
                {
                    _logger.LogWarning("UserRoles with ID {UserRolesId} not found", id);
                    return NotFound(new ApiResponse<object>($"UserRoles with ID {id} not found"));
                }

                var userRolesDto = _mapper.Map<UserRolesdto>(userRoles);

                _logger.LogInformation("Successfully retrieved userRoles {UserRolesId}", id);

                return Ok(new ApiResponse<UserRolesdto>(userRolesDto, "UserRoles retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching userRoles {UserId}", id);
                throw;
            }
        }

    }
}
