using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.TransactionTypes;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// TransationTypes management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class TransactionTypesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TransactionTypesController> _logger;
        private readonly IValidator<CreateTransactionTypesdto> _createValidator;
        private readonly IValidator<UpdateTransactionTypesdto> _updateValidator;

        public TransactionTypesController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<TransactionTypesController> logger,
            IValidator<CreateTransactionTypesdto> createValidator,
            IValidator<UpdateTransactionTypesdto> updateValidator)
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
        /// <returns>Paginated list of transactionatypes</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<TransactionTypesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<TransactionTypesdto>>> GetTransactionTypes(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching transactionTypess for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (transactionTypes, totalCount) = await _unitOfWork.TransactionTypes.GetTransactionTypesPagedAsync(
                    tenantId, searchTerm, pageNumber, pageSize);

                var transactionTypeDtos = _mapper.Map<List<TransactionTypesdto>>(transactionTypes);

                var response = new PagedResponse<TransactionTypesdto>(transactionTypeDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} transactions out of {TotalCount} for tenant {TenantId}",
                    transactionTypeDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching transactionTypes for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get transaction by ID
        /// </summary>
        /// <param name="id">Transaction ID</param>
        /// <returns>Transaction details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<TransactionTypesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<TransactionTypesdto>>> GetTransactionTypesById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching transaction with ID: {TransactionId}", id);

                var transactionType = await _unitOfWork.TransactionTypes.GetByIdAsync(id);

                if (transactionType == null)
                {
                    _logger.LogWarning("Transaction with ID {TransactionId} not found", id);
                    return NotFound(new ApiResponse<object>($"Transaction with ID {id} not found"));
                }

                var transactionTypeDto = _mapper.Map<TransactionTypesdto>(transactionType);

                _logger.LogInformation("Successfully retrieved transcation {TransactionId}", id);

                return Ok(new ApiResponse<TransactionTypesdto>(transactionTypeDto, "Transaction retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching customer {CustomerId}", id);
                throw;
            }
        }

        /// <summary>
        /// Get transaction by type code
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="customerCode">Type code</param>
        /// <returns>Type details</returns>
        [HttpGet("by-code/{typeCode}")]
        [ProducesResponseType(typeof(ApiResponse<TransactionTypesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<TransactionTypesdto>>> GetTypeByCode(
            [FromQuery] Guid tenantId,
            string typeCode)
        {
            try
            {
                _logger.LogInformation("Fetching transactionType with code {typeCode} for tenant {TenantId}",
                    typeCode, tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var transactionType = await _unitOfWork.TransactionTypes.GetByTypeCodeAsync(tenantId, typeCode);

                if (transactionType == null)
                {
                    _logger.LogWarning("Transaction with code {TypeCode} not found for tenant {TenantId}",
                        typeCode, tenantId);
                    return NotFound(new ApiResponse<object>($"Transaction with code {typeCode} not found"));
                }

                var transactionTypeDto = _mapper.Map<TransactionTypesdto>(transactionType);

                _logger.LogInformation("Successfully retrieved transaction with code {TypeCode}", typeCode);

                return Ok(new ApiResponse<TransactionTypesdto>(transactionTypeDto, "Transaction retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching transaction with code {TypeCode}", typeCode);
                throw;
            }
        }

        /// <summary>
        /// Create a new transactionType
        /// </summary>
        /// <param name="createTransactionTypesdto">transaction creation data</param>
        /// <returns>Created transaction details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<TransactionTypesdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<TransactionTypesdto>>> CreateTransactionTypes([FromBody] CreateTransactionTypesdto createTransactionTypesdto)
        {
            try
            {
                _logger.LogInformation("Creating new transaction with code {TypeCode} for tenant {TenantId}",
                    createTransactionTypesdto.TypeCode, createTransactionTypesdto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createTransactionTypesdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for transaction creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Check if type code is unique
                var isUnique = await _unitOfWork.TransactionTypes.IsTypeCodeUniqueAsync(
                    createTransactionTypesdto.TenantId, createTransactionTypesdto.TypeCode);

                if (!isUnique)
                {
                    _logger.LogWarning("Transaction code {TypeCode} already exists for tenant {TenantId}",
                        createTransactionTypesdto.TypeCode, createTransactionTypesdto.TenantId);
                    return BadRequest(new ApiResponse<object>("Type code already exists"));
                }

                // Map DTO to entity
                var transactionType = _mapper.Map<TransactionTypes>(createTransactionTypesdto);
                transactionType.TransactionId = Guid.NewGuid();
                transactionType.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.TransactionTypes.AddAsync(transactionType);
                await _unitOfWork.SaveChangesAsync();

                var transactionTypeDto = _mapper.Map<TransactionTypesdto>(transactionType);

                _logger.LogInformation("Successfully created transaction {TransactionId} with code {TypeCode}",
                    transactionType.TransactionId, transactionType.TypeCode);

                return CreatedAtAction(
                    nameof(GetTransactionTypesById),
                    new { id = transactionType.TransactionId },
                    new ApiResponse<TransactionTypesdto>(transactionTypeDto, "Transaction created successfully"));
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
        [ProducesResponseType(typeof(ApiResponse<TransactionTypesdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<TransactionTypesdto>>> UpdateTransactionTypes(
            Guid id,
            [FromBody] UpdateTransactionTypesdto updateTransactionTypesdto)
        {
            try
            {
                _logger.LogInformation("Updating transaction {TransactionId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateTransactionTypesdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for transaction update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing transaction
                var existingTransactionType = await _unitOfWork.TransactionTypes.GetByIdAsync(id);
                if (existingTransactionType == null)
                {
                    _logger.LogWarning("Transaction {TransactionId} not found", id);
                    return NotFound(new ApiResponse<object>($"Transaction with ID {id} not found"));
                }

                // Update only provided fields

                if (!string.IsNullOrWhiteSpace(updateTransactionTypesdto.TypeName))
                    existingTransactionType.TypeName = updateTransactionTypesdto.TypeName;

                if (updateTransactionTypesdto.IsActive.HasValue)
                    existingTransactionType.IsActive = updateTransactionTypesdto.IsActive.Value;


                _unitOfWork.TransactionTypes.Update(existingTransactionType);
                await _unitOfWork.SaveChangesAsync();

                var transactionDto = _mapper.Map<TransactionTypesdto>(existingTransactionType);

                _logger.LogInformation("Successfully updated transaction {TransactionId}", id);

                return Ok(new ApiResponse<TransactionTypesdto>(transactionDto, "Transaction updated successfully"));
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
        public async Task<ActionResult<ApiResponse<object>>> DeletetransactionTypes(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting transaction {TransactionId}", id);

                var transactionType = await _unitOfWork.TransactionTypes.GetByIdAsync(id);
                if (transactionType == null)
                {
                    _logger.LogWarning("Transaction {TransactionId} not found", id);
                    return NotFound(new ApiResponse<object>($"Transaction with ID {id} not found"));
                }

                // Soft delete
                transactionType.IsActive = false;

                _unitOfWork.TransactionTypes.Update(transactionType);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted transactiontype {TransactionId}", id);

                return Ok(new ApiResponse<object>(null, "Transaction deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting transaction {TransactionId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search transactionTypes by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching transactions</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<TransactionTypesdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<TransactionTypesdto>>>> SearchtransactionTypes(
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

                var transactionTypes = await _unitOfWork.TransactionTypes.SearchTransactionTypesAsync(tenantId, searchTerm);
                var transactionTypeDtos = _mapper.Map<List<TransactionTypesdto>>(transactionTypes);

                _logger.LogInformation("Found {Count} transactiontypes matching search term", transactionTypeDtos.Count);

                return Ok(new ApiResponse<List<TransactionTypesdto>>(transactionTypeDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching transactiontypes");
                throw;
            }
        }

        /// <summary>
        /// Get active transactionTypes for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active transactions</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<TransactionTypesdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<TransactionTypesdto>>>> GetActiveTransactionTypes([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active transactionTypes for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var transactionTypes = await _unitOfWork.TransactionTypes.GetActiveTransactionTypesAsync(tenantId);
                var transactionTypeDtos = _mapper.Map<List<TransactionTypesdto>>(transactionTypes);

                _logger.LogInformation("Successfully retrieved {Count} active transactions", transactionTypeDtos.Count);

                return Ok(new ApiResponse<List<TransactionTypesdto>>(transactionTypeDtos, "Active transactions retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active transactionTypes for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}