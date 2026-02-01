using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.AccountTypes;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// accountTypes management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class AccountTypesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountTypesController> _logger;
        private readonly IValidator<CreateAccountTypesdto> _createValidator;
        private readonly IValidator<UpdateAccountTypesdto> _updateValidator;

        public AccountTypesController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AccountTypesController> logger,
            IValidator<CreateAccountTypesdto> createValidator,
            IValidator<UpdateAccountTypesdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all accountTypes with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of accountTypes</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<AccountTypesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<AccountTypesdto>>> GetAccountTypes(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching accountTypes for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (accountTypes, totalCount) = await _unitOfWork.AccountTypes.GetAccountTypesPagedAsync(
                    tenantId, searchTerm, pageNumber, pageSize);

                var accountTypeDtos = _mapper.Map<List<AccountTypesdto>>(accountTypes);

                var response = new PagedResponse<AccountTypesdto>(accountTypeDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} accountType out of {TotalCount} for tenant {TenantId}",
                    accountTypeDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching accountType for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get accountType by ID
        /// </summary>
        /// <param name="id">AccountType ID</param>
        /// <returns>AccountType details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AccountTypesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<AccountTypesdto>>> GetAccountTypesById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching accountType with ID: {AccountTypeId}", id);

                var accountType = await _unitOfWork.AccountTypes.GetByIdAsync(id);

                if (accountType == null)
                {
                    _logger.LogWarning("AccountType with ID {AccountTypeId} not found", id);
                    return NotFound(new ApiResponse<object>($"AccountType with ID {id} not found"));
                }

                var accountTypeDto = _mapper.Map<AccountTypesdto>(accountType);

                _logger.LogInformation("Successfully retrieved accountType {AccountTypeId}", id);

                return Ok(new ApiResponse<AccountTypesdto>(accountTypeDto, "AccountType retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching accountType {accountTypeId}", id);
                throw;
            }
        }

        /// <summary>
        /// Get AccountType by accountType code
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="accountTypeCode">account code</param>
        /// <returns>Account details</returns>
        [HttpGet("by-code/{accountTypeCode}")]
        [ProducesResponseType(typeof(ApiResponse<AccountTypesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<AccountTypesdto>>> GetAccountTypesByCode(
            [FromQuery] Guid tenantId,
            string accountTypeCode)
        {
            try
            {
                _logger.LogInformation("Fetching accountType with code {AccountTypeCode} for tenant {TenantId}",
                    accountTypeCode, tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var accountType = await _unitOfWork.AccountTypes.GetByAccountTypesCodeAsync(tenantId, accountTypeCode);

                if (accountType == null)
                {
                    _logger.LogWarning("AccountType with code {AccountTypeCode} not found for tenant {TenantId}",
                        accountTypeCode, tenantId);
                    return NotFound(new ApiResponse<object>($"AccountType with code {accountTypeCode} not found"));
                }

                var accountTypeDto = _mapper.Map<AccountTypesdto>(accountType);

                _logger.LogInformation("Successfully retrieved account with code {AccountTypeCode}", accountTypeCode);

                return Ok(new ApiResponse<AccountTypesdto>(accountTypeDto, "AccountType retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching accountType with code {AccountTypeCode}", accountTypeCode);
                throw;
            }
        }

        /// <summary>
        /// Create a new accountTypes
        /// </summary>
        /// <param name="createAccountTypesdto">AccountType creation data</param>
        /// <returns>Created accountType details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AccountTypesdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<AccountTypesdto>>> CreateAccountTypes([FromBody] CreateAccountTypesdto createAccountTypesdto)
        {
            try
            {
                _logger.LogInformation("Creating new accountType with code {AccountTypeCode} for tenant {TenantId}",
                    createAccountTypesdto.AccountTypeCode, createAccountTypesdto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createAccountTypesdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Check if customer code is unique
                var isUnique = await _unitOfWork.AccountTypes.IsAccountTypesCodeUniqueAsync(
                    createAccountTypesdto.TenantId, createAccountTypesdto.AccountTypeCode);

                if (!isUnique)
                {
                    _logger.LogWarning("AccountType code {accountTypeCode} already exists for tenant {TenantId}",
                        createAccountTypesdto.AccountTypeCode, createAccountTypesdto.TenantId);
                    return BadRequest(new ApiResponse<object>("AccountType code already exists"));
                }

                // Map DTO to entity
                var accountType = _mapper.Map<AccountTypes>(createAccountTypesdto);
                accountType.AccountTypeId = Guid.NewGuid();
                accountType.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.AccountTypes.AddAsync(accountType);
                await _unitOfWork.SaveChangesAsync();

                var accountTypeDto = _mapper.Map<AccountTypesdto>(accountType);

                _logger.LogInformation("Successfully created accountType {AccountTypeId} with code {AccountTypeCode}",
                    accountType.AccountTypeId, accountType.AccountTypeCode);

                return CreatedAtAction(
                    nameof(GetAccountTypesById),
                    new { id = accountType.AccountTypeId },
                    new ApiResponse<AccountTypesdto>(accountTypeDto, "AccountType created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating account");
                throw;
            }
        }

        /// <summary>
        /// Update an existing accountType
        /// </summary>
        /// <param name="id">AccountType ID</param>
        /// <param name="updateAccountTypesdto">AccountType update data</param>
        /// <returns>Updated account details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AccountTypesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<AccountTypesdto>>> UpdateAccountTypes(
            Guid id,
            [FromBody] UpdateAccountTypesdto updateAccountTypesdto)
        {
            try
            {
                _logger.LogInformation("Updating accountTyoe {AccountTypeId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateAccountTypesdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing account
                var existingAccountType = await _unitOfWork.AccountTypes.GetByIdAsync(id);
                if (existingAccountType == null)
                {
                    _logger.LogWarning("AccountType {AccountTypeId} not found", id);
                    return NotFound(new ApiResponse<object>($"AccountType with ID {id} not found"));
                }

                // Update only provided fields
                
                if (!string.IsNullOrWhiteSpace(updateAccountTypesdto.AccountTypeName))
                    existingAccountType.AccountTypeName = updateAccountTypesdto.AccountTypeName;

               if (updateAccountTypesdto.IsActive.HasValue)
                    existingAccountType.IsActive = updateAccountTypesdto.IsActive.Value;

               _unitOfWork.AccountTypes.Update(existingAccountType);
                await _unitOfWork.SaveChangesAsync();

                var accountTypeDto = _mapper.Map<AccountTypesdto>(existingAccountType);

                _logger.LogInformation("Successfully updated accountType {accountTypeId}", id);

                return Ok(new ApiResponse<AccountTypesdto>(accountTypeDto, "AccountType updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating accountType {AccountTypeId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a accountType (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">AccountType ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteAccountTypes(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting accountType {AccountTypeId}", id);

                var accountType = await _unitOfWork.AccountTypes.GetByIdAsync(id);
                if (accountType == null)
                {
                    _logger.LogWarning("AccountType {AccountTypeId} not found", id);
                    return NotFound(new ApiResponse<object>($"AccountType with ID {id} not found"));
                }

                // Soft delete
                accountType.IsActive = false;

                _unitOfWork.AccountTypes.Update(accountType);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted accountType {AccountTypeId}", id);

                return Ok(new ApiResponse<object>(null, "AccountType deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting accountType {AccountTypeId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search account by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching account</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<AccountTypesdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<AccountTypesdto>>>> SearchAccountTypes(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching accountTypes for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var accountType = await _unitOfWork.AccountTypes.SearchAccountTypesAsync(tenantId, searchTerm);
                var accountTypeDtos = _mapper.Map<List<AccountTypesdto>>(accountType);

                _logger.LogInformation("Found {Count} accountType matching search term", accountTypeDtos.Count);

                return Ok(new ApiResponse<List<AccountTypesdto>>(accountTypeDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching accountType");
                throw;
            }
        }

        /// <summary>
        /// Get active accountTypes for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active account</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<AccountTypesdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<AccountTypesdto>>>> GetActiveAccountTypes([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active accountType for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var accountType = await _unitOfWork.AccountTypes.GetActiveAccountTypesAsync(tenantId);
                var accountTypeDtos = _mapper.Map<List<AccountTypesdto>>(accountType);

                _logger.LogInformation("Successfully retrieved {Count} active accountTypes", accountTypeDtos.Count);

                return Ok(new ApiResponse<List<AccountTypesdto>>(accountTypeDtos, "Active accountType retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active accountTypes for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}