using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Users;
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
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UserController> _logger;
        private readonly IValidator<CreateUserdto> _createValidator;
        private readonly IValidator<UpdateUserdto> _updateValidator;

        public UserController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<UserController> logger,
            IValidator<CreateUserdto> createValidator,
            IValidator<UpdateUserdto> updateValidator)
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
        /// <param name="userId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        

        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of users</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<Userdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<Userdto>>> GetUsers(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching users for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (users, totalCount) = await _unitOfWork.Users.GetUsersPagedAsync(
                    tenantId, searchTerm, pageNumber, pageSize);

                var userDtos = _mapper.Map<List<Userdto>>(users);

                var response = new PagedResponse<Userdto>(userDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} users out of {TotalCount} for user {TenantId}",
                    userDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching users for user {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Userdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Userdto>>> GetUserById(Guid id)
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

                var userDto = _mapper.Map<Userdto>(user);

                _logger.LogInformation("Successfully retrieved user {UserId}", id);

                return Ok(new ApiResponse<Userdto>(userDto, "User retrieved successfully"));
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
        /// <param name="createUserDto">User creation data</param>
        /// <returns>Created user details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Userdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Userdto>>> CreateUser([FromBody] CreateUserdto createUserDto)
        {
            try
            {
                _logger.LogInformation("Creating new user with id {TenantId} ",
                    createUserDto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createUserDto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for user creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                

                // Map DTO to entity
                var user = _mapper.Map<User>(createUserDto);
                user.TenantId = Guid.NewGuid();
                user.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var userDto = _mapper.Map<Userdto>(user);

                _logger.LogInformation("Successfully created user {TenantId} ",
                    user.TenantId);

                return CreatedAtAction(
                    nameof(GetUserById),
                    new { id = user.TenantId },
                    new ApiResponse<Userdto>(userDto, "User created successfully"));
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
        /// <param name="id">Tenant ID</param>
        /// <param name="updateUserDto">User update data</param>
        /// <returns>Updated user details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Userdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Userdto>>> UpdateTenant(
            Guid id,
            [FromBody] UpdateUserdto updateUserDto)
        {
            try
            {
                _logger.LogInformation("Updating user {TenantId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateUserDto);
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
                    _logger.LogWarning("Users {TenantId} not found", id);
                    return NotFound(new ApiResponse<object>($"User with ID {id} not found"));
                }

                // Update only provided fields
                if (!string.IsNullOrWhiteSpace(updateUserDto.UserName))
                    existingUser.UserName = updateUserDto.UserName;


                if (!string.IsNullOrWhiteSpace(updateUserDto.Email))
                    existingUser.Email = updateUserDto.Email;

                if (!string.IsNullOrWhiteSpace(updateUserDto.FirstName))
                    existingUser.FirstName = updateUserDto.FirstName;

                if (!string.IsNullOrWhiteSpace(updateUserDto.LastName))
                    existingUser.LastName = updateUserDto.LastName;

                if (!string.IsNullOrWhiteSpace(updateUserDto.PhoneNumber))
                    existingUser.PhoneNumber = updateUserDto.PhoneNumber;

                
               if (updateUserDto.IsActive.HasValue)
                    existingUser.IsActive = updateUserDto.IsActive.Value;


                existingUser.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Users.Update(existingUser);
                await _unitOfWork.SaveChangesAsync();

                var userDto = _mapper.Map<Userdto>(existingUser);

                _logger.LogInformation("Successfully updated User {TenantId}", id);

                return Ok(new ApiResponse<Userdto>(userDto, "User updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user {TenantId}", id);
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
                _logger.LogInformation("Deleting user {TenantId}", id);

                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User {TenantId} not found", id);
                    return NotFound(new ApiResponse<object>($"User with ID {id} not found"));
                }

                // Soft delete
                user.IsActive = false;
                user.ModifiedDate = DateTime.UtcNow;


                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted tenant {TenantId}", id);

                return Ok(new ApiResponse<object>(null, "Tenant deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting tenant {TenantId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search users by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching tenant</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<Userdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Userdto>>>> SearchUsers(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching users for user {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var users = await _unitOfWork.Users.SearchUsersAsync(tenantId, searchTerm);
                var userDtos = _mapper.Map<List<Userdto>>(users);

                _logger.LogInformation("Found {Count} users matching search term", userDtos.Count);

                return Ok(new ApiResponse<List<Userdto>>(userDtos, "Search completed successfully"));
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
        [ProducesResponseType(typeof(ApiResponse<List<Userdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Userdto>>>> GetActiveUsers([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active users for user {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var user = await _unitOfWork.Users.GetActiveUsersAsync(tenantId);
                var userDtos = _mapper.Map<List<Userdto>>(user);

                _logger.LogInformation("Successfully retrieved {Count} active users", userDtos.Count);

                return Ok(new ApiResponse<List<Userdto>>(userDtos, "Active users retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active user for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}