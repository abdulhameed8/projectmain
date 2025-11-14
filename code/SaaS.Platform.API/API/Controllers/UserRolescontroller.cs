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
    /// Tenant management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class UserRolesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UserRolesController> _logger;
        private readonly IValidator<CreateUserRolesdto> _createValidator;
        private readonly IValidator<UpdateUserRolesdto> _updateValidator;

        public UserRolesController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UserController> logger,
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
        /// Get all user with optional pagination and filtering
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="searchTerm">Search term for filtering</param>


        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of users</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<UserRolesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<UserRolesdto>>> GetUsersRoles(
            [FromQuery] Guid userId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching users for user {UserId} - Page: {PageNumber}, Size: {PageSize}",
                    userId, pageNumber, pageSize);

                if (userId == Guid.Empty)
                {
                    _logger.LogWarning("Invalid user ID provided");
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                {
                    _logger.LogWarning("Invalid pagination parameters: PageNumber={PageNumber}, PageSize={PageSize}",
                        pageNumber, pageSize);
                    return BadRequest(new ApiResponse<object>("Invalid pagination parameters"));
                }

                var (usersRoles, totalCount) = await _unitOfWork.UsersRoles.GetUsersPagedAsync(
                    userId, searchTerm, pageNumber, pageSize);

                var userRolesDtos = _mapper.Map<List<UserRolesdto>>(usersRoles);

                var response = new PagedResponse<UserRolesdto>(userRolesDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} users out of {TotalCount} for user {UserId}",
                    userRolesDtos.Count, totalCount, userId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching users for user {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Get userRoles by ID
        /// </summary>
        /// <param name="id">UserRoles ID</param>
        /// <returns>User details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserRolesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<UserRolesdto>>> GetUserRolesById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching user with ID: {UserId}", id);

                var user = await _unitOfWork.Users.GetByIdAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", id);
                    return NotFound(new ApiResponse<object>($"User with ID {id} not found"));
                }

                var userRolesDto = _mapper.Map<UserRolesdto>(user);

                _logger.LogInformation("Successfully retrieved user {UserId}", id);

                return Ok(new ApiResponse<UserRolesdto>(userRolesDto, "User retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user {UserId}", id);
                throw;
            }
        }



        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="createUserRolesDto">User creation data</param>
        /// <returns>Created user details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UserRolesdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<UserRolesdto>>> CreateUser([FromBody] CreateUserRolesdto createUserRolesDto)
        {
            try
            {
                _logger.LogInformation("Creating new user with id {TenantId} ",
                    createUserRolesDto.UserId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createUserRolesDto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for user creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }



                // Map DTO to entity
                var user = _mapper.Map<User>(createUserRolesDto);
                user.userId = Guid.NewGuid();
                user.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var userRolesDto = _mapper.Map<UserRolesdto>(user);

                _logger.LogInformation("Successfully created user {UserId} ",
                    user.TenantId);

                return CreatedAtAction(
                    nameof(GetUserRolesById),
                    new { id = user.UserId },
                    new ApiResponse<UserRolesdto>(userRolesDto, "User created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user");
                throw;
            }
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="updateUserRolesDto">UserRoles update data</param>
        /// <returns>Updated user details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserRolesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<UserRolesdto>>> UpdateUserroles(
            Guid id,
            [FromBody] UpdateUserRolesdto updateUserRolesDto)
        {
            try
            {
                _logger.LogInformation("Updating user {UserId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateUserRolesDto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for user update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing user
                var existingUser = await _unitOfWork.Users.GetByIdAsync(id);
                if (existingUser == null)
                {
                    _logger.LogWarning("Users {UserId} not found", id);
                    return NotFound(new ApiResponse<object>($"User with ID {id} not found"));
                }

                // Update only provided fields
               

               
                _unitOfWork.Users.Update(existingUser);
                await _unitOfWork.SaveChangesAsync();

                var userRolesDto = _mapper.Map<UserRolesdto>(existingUser);

                _logger.LogInformation("Successfully updated User {UserId}", id);

                return Ok(new ApiResponse<UserRolesdto>(userRolesDto, "User updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user {UserId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a user (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteUser(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting user {UserId}", id);

                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", id);
                    return NotFound(new ApiResponse<object>($"User with ID {id} not found"));
                }

                // Soft delete
                user.IsActive = false;
                user.ModifiedDate = DateTime.UtcNow;


                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted UserRoles {UserId}", id);

                return Ok(new ApiResponse<object>(null, "UserRoles deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting userRoles {UserId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search users by search term
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching tenant</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<UserRolesdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<UserRolesdto>>>> SearchUsersRoles(
            [FromQuery] Guid userId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching users for user {UserId} with term: {SearchTerm}",
                    userId, searchTerm);

                if (userId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("User ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var users = await _unitOfWork.Users.SearchUsersAsync(userId, searchTerm);
                var userRolesDtos = _mapper.Map<List<UserRolesdto>>(users);

                _logger.LogInformation("Found {Count} users matching search term", userRolesDtos.Count);

                return Ok(new ApiResponse<List<UserRolesdto>>(userRolesDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching users");
                throw;
            }
        }

        /// <summary>
        /// Get active tenant for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active users</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<UserRolesdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<UserRolesdto>>>> GetActiveUsers([FromQuery] Guid userId)
        {
            try
            {
                _logger.LogInformation("Fetching active users for user {UserId}", userId);

                if (userId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("User ID is required"));
                }

                var user = await _unitOfWork.Users.GetActiveUsersAsync(userId);
                var userRolesDtos = _mapper.Map<List<UserRolesdto>>(user);

                _logger.LogInformation("Successfully retrieved {Count} active users", userRolesDtos.Count);

                return Ok(new ApiResponse<List<UserRolesdto>>(userRolesDtos, "Active users retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active user for tenant {UserId}", userId);
                throw;
            }
        }
    }
}