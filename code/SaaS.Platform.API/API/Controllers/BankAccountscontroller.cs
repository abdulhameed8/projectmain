using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Bankaccounts;
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
    public class BankAccountsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BankAccountsController> _logger;
        private readonly IValidator<CreateBankAccountsdto> _createValidator;
        private readonly IValidator<UpdateBankaccountsdto> _updateValidator;

        public BankAccountsController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<BankAccountsController> logger,
            IValidator<CreateBankAccountsdto> createValidator,
            IValidator<UpdateBankaccountsdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all BankAccounts with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="status">Customer status filter</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of BankAccount</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<BankAccountsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<BankAccountsdto>>> GetBankAccounts(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? status = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching bankAccounts for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (bankAccounts, totalCount) = await _unitOfWork.BankAccounts.GetBankAccountsPagedAsync(
                    tenantId, searchTerm, status,  pageNumber, pageSize);

                var bankAccountDtos = _mapper.Map<List<BankAccountsdto>>(bankAccounts);

                var response = new PagedResponse<BankAccountsdto>(bankAccountDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} bankAccount out of {TotalCount} for tenant {TenantId}",
                    bankAccountDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching bankAccounts for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get Account by ID
        /// </summary>
        /// <param name="id">account ID</param>
        /// <returns>BankAccount details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<BankAccountsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<BankAccountsdto>>> GetBankAccountById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching bankAccount with ID: {AccountId}", id);

                var bankAccount = await _unitOfWork.BankAccounts.GetByIdAsync(id);

                if (bankAccount == null)
                {
                    _logger.LogWarning("BankAccount with ID {AccountId} not found", id);
                    return NotFound(new ApiResponse<object>($"Account with ID {id} not found"));
                }

                var bankAccountDto = _mapper.Map<BankAccountsdto>(bankAccount);

                _logger.LogInformation("Successfully retrieved account {AccountId}", id);

                return Ok(new ApiResponse<BankAccountsdto>(bankAccountDto, "BankAccount retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching account {AccountId}", id);
                throw;
            }
        }

        
        /// <summary>
        /// Create a new bankAccount
        /// </summary>
        /// <param name="createBankAccountsdto">BankAccount creation data</param>
        /// <returns>Created BankAccount details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<BankAccountsdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<BankAccountsdto>>> CreateBankAccountsdto([FromBody] CreateBankAccountsdto createBankAccountsdto)
        {
            try
            {
                _logger.LogInformation("Creating new bankAccount with  tenant {TenantId}",
                     createBankAccountsdto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createBankAccountsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                

                // Map DTO to entity
                var bankAccount = _mapper.Map<BankAccounts>(createBankAccountsdto);
                bankAccount.AccountId = Guid.NewGuid();
                bankAccount.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.BankAccounts.AddAsync(bankAccount);
                await _unitOfWork.SaveChangesAsync();

                var bankAccountDto = _mapper.Map<BankAccountsdto>(bankAccount);

                _logger.LogInformation("Successfully created bankAccount {AccountId}",
                    bankAccount.AccountId);

                return CreatedAtAction(
                    nameof(GetBankAccountById),
                    new { id = bankAccount.AccountId },
                    new ApiResponse<BankAccountsdto>(bankAccountDto, "BankAccount created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating bankAccount");
                throw;
            }
        }

        /// <summary>
        /// Update an existing bankAccount
        /// </summary>
        /// <param name="id">Account ID</param>
        /// <param name="updateBankAccountsdto">BankAccounts update data</param>
        /// <returns>Updated banAccount details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<BankAccountsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<BankAccountsdto>>> UpdateBankAccount(
            Guid id,
            [FromBody] UpdateBankaccountsdto updateBankAccountsdto)
        {
            try
            {
                _logger.LogInformation("Updating bankAccount {AccountId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateBankAccountsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing bankAccount
                var existingBankAccounts = await _unitOfWork.BankAccounts.GetByIdAsync(id);
                if (existingBankAccounts == null)
                {
                    _logger.LogWarning("BankAccount {AccountId} not found", id);
                    return NotFound(new ApiResponse<object>($"BankAccount with ID {id} not found"));
                }

                // Update only provided fields

                if (!string.IsNullOrWhiteSpace(updateBankAccountsdto.AccountName))
                    existingBankAccounts.AccountName = updateBankAccountsdto.AccountName;

               
                if (!string.IsNullOrWhiteSpace(updateBankAccountsdto.AccountStatus))
                    existingBankAccounts.AccountStatus = updateBankAccountsdto.AccountStatus;


                existingBankAccounts.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.BankAccounts.Update(existingBankAccounts);
                await _unitOfWork.SaveChangesAsync();

                var bankAccountDto = _mapper.Map<BankAccountsdto>(existingBankAccounts);

                _logger.LogInformation("Successfully updated BankAccount {AccountId}", id);

                return Ok(new ApiResponse<BankAccountsdto>(bankAccountDto, "BankAccount updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating bankAccount {AccountId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a bankAccount (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Account ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteBankAccount(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting bankAccount {AccountId}", id);

                var bankAccount = await _unitOfWork.BankAccounts.GetByIdAsync(id);
                if (bankAccount == null)
                {
                    _logger.LogWarning("BankAccounts {AccountId} not found", id);
                    return NotFound(new ApiResponse<object>($"BankAccount with ID {id} not found"));
                }

                // Soft delete
                bankAccount.AccountStatus = "Inactive";
                bankAccount.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.BankAccounts.Update(bankAccount);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted bankAccount {AccountId}", id);

                return Ok(new ApiResponse<object>(null, "Account deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting account {AccountId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search BankAccount by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching bankAccount</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<BankAccountsdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<BankAccountsdto>>>> SearchBankAccounts(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching bankAccount for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var bankAccount = await _unitOfWork.BankAccounts.SearchBankAccountsAsync(tenantId, searchTerm);
                var bankAccountDtos = _mapper.Map<List<BankAccountsdto>>(bankAccount);

                _logger.LogInformation("Found {Count} bankAccounts matching search term", bankAccountDtos.Count);

                return Ok(new ApiResponse<List<BankAccountsdto>>(bankAccountDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching bankAccounts");
                throw;
            }
        }

        /// <summary>
        /// Get active bankaccount for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active bankaccount</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<BankAccountsdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<BankAccountsdto>>>> GetActiveBankAccounts([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active bankaccount for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var bankAccounts = await _unitOfWork.BankAccounts.GetActiveBankAccountsAsync(tenantId);
                var bankAccountDtos = _mapper.Map<List<BankAccountsdto>>(bankAccounts);

                _logger.LogInformation("Successfully retrieved {Count} active bankAccounts", bankAccountDtos.Count);

                return Ok(new ApiResponse<List<BankAccountsdto>>(bankAccountDtos, "Active bankAccount retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active bankAccounts for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}