using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Transaction;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// Transations management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class TransactionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TransactionsController> _logger;
        private readonly IValidator<CreateTransactionsdto> _createValidator;
        private readonly IValidator<UpdateTransactionsdto> _updateValidator;

        public TransactionsController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<TransactionsController> logger,
            IValidator<CreateTransactionsdto> createValidator,
            IValidator<UpdateTransactionsdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all transactions with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of transactionas</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<Transactionsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<Transactionsdto>>> GetTransactions(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching transactions for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (transactions, totalCount) = await _unitOfWork.Transactions.GetTransactionsPagedAsync(
                    tenantId, searchTerm, pageNumber, pageSize);

                var transactionDtos = _mapper.Map<List<Transactionsdto>>(transactions);

                var response = new PagedResponse<Transactionsdto>(transactionDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} transactions out of {TotalCount} for tenant {TenantId}",
                    transactionDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching transactions for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get transaction by ID
        /// </summary>
        /// <param name="id">Transaction ID</param>
        /// <returns>Transaction details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Transactionsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Transactionsdto>>> GetTransactionsById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching transaction with ID: {TransactionId}", id);

                var transaction = await _unitOfWork.Transactions.GetByIdAsync(id);

                if (transaction == null)
                {
                    _logger.LogWarning("Transaction with ID {TransactionId} not found", id);
                    return NotFound(new ApiResponse<object>($"Transaction with ID {id} not found"));
                }

                var transactionDto = _mapper.Map<Transactionsdto>(transaction);

                _logger.LogInformation("Successfully retrieved transcation {TransactionId}", id);

                return Ok(new ApiResponse<Transactionsdto>(transactionDto, "Transaction retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching transaction {TransactionId}", id);
                throw;
            }
        }

        
        /// <summary>
        /// Create a new transactionType
        /// </summary>
        /// <param name="createTransactionTypesdto">transaction creation data</param>
        /// <returns>Created transaction details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Transactionsdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Transactionsdto>>> CreateTransactions([FromBody] CreateTransactionsdto createTransactionsdto)
        {
            try
            {
                _logger.LogInformation("Creating new transaction  for tenant {TenantId}",
                     createTransactionsdto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createTransactionsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for transaction creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

               

                // Map DTO to entity
                var transaction = _mapper.Map<Transactions>(createTransactionsdto);
                transaction.TransactionId = Guid.NewGuid();
                transaction.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.Transactions.AddAsync(transaction);
                await _unitOfWork.SaveChangesAsync();

                var transactionDto = _mapper.Map<Transactionsdto>(transaction);

                _logger.LogInformation("Successfully created transaction {TransactionId} ",
                    transaction.TransactionId);

                return CreatedAtAction(
                    nameof(GetTransactionsById),
                    new { id = transaction.TransactionId },
                    new ApiResponse<Transactionsdto>(transactionDto, "Transaction created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating transaction");
                throw;
            }
        }

        /// <summary>
        /// Update an existing transaction
        /// </summary>
        /// <param name="id">Transaction ID</param>
        /// <param name="updateTransactionTypesdto">Transaction update data</param>
        /// <returns>Updated transcation details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Transactionsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Transactionsdto>>> UpdateTransactions(
            Guid id,
            [FromBody] UpdateTransactionsdto updateTransactionsdto)
        {
            try
            {
                _logger.LogInformation("Updating transaction {TransactionId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateTransactionsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for transaction update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing transaction
                var existingTransaction = await _unitOfWork.Transactions.GetByIdAsync(id);
                if (existingTransaction == null)
                {
                    _logger.LogWarning("Transaction {TransactionId} not found", id);
                    return NotFound(new ApiResponse<object>($"Transaction with ID {id} not found"));
                }

                // Update only provided fields

               

                _unitOfWork.Transactions.Update(existingTransaction);
                await _unitOfWork.SaveChangesAsync();

                var transactionDto = _mapper.Map<Transactionsdto>(existingTransaction);

                _logger.LogInformation("Successfully updated transaction {TransactionId}", id);

                return Ok(new ApiResponse<Transactionsdto>(transactionDto, "Transaction updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating transaction {TransactionId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a transaction (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Transaction ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> Deletetransactions(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting transaction {TransactionId}", id);

                var transaction = await _unitOfWork.Transactions.GetByIdAsync(id);
                if (transaction == null)
                {
                    _logger.LogWarning("Transaction {TransactionId} not found", id);
                    return NotFound(new ApiResponse<object>($"Transaction with ID {id} not found"));
                }

                // Soft delete

                _unitOfWork.Transactions.Update(transaction);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted transaction {TransactionId}", id);

                return Ok(new ApiResponse<object>(null, "Transaction deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting transaction {TransactionId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search transaction by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching transactions</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<Transactionsdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Transactionsdto>>>> Searchtransactions(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching transactionTypess for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var transactions = await _unitOfWork.Transactions.SearchTransactionsAsync(tenantId, searchTerm);
                var transactionDtos = _mapper.Map<List<Transactionsdto>>(transactions);

                _logger.LogInformation("Found {Count} transactions matching search term", transactionDtos.Count);

                return Ok(new ApiResponse<List<Transactionsdto>>(transactionDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching transactionts");
                throw;
            }
        }

        /// <summary>
        /// Get active transactions for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active transactions</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<Transactionsdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Transactionsdto>>>> GetActiveTransactions([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active transactions for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var transactions = await _unitOfWork.Transactions.GetActiveTransactionsAsync(tenantId);
                var transactionDtos = _mapper.Map<List<Transactionsdto>>(transactions);

                _logger.LogInformation("Successfully retrieved {Count} active transactions", transactionDtos.Count);

                return Ok(new ApiResponse<List<Transactionsdto>>(transactionDtos, "Active transactions retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active transaction for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}