using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Cards;
using SaaS.Platform.API.Application.DTOs.Loans;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// Loans management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class LoansController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<LoansController> _logger;
        private readonly IValidator<CreateLoansdto> _createValidator;
        private readonly IValidator<UpdateLoansdto> _updateValidator;

        public LoansController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<LoansController> logger,
            IValidator<CreateLoansdto> createValidator,
            IValidator<UpdateLoansdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all loans with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="status">loan status filter</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of loans</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<Loansdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<Loansdto>>> GetLoans(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? status = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching loans for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (loans, totalCount) = await _unitOfWork.Loans.GetLoansPagedAsync(
                    tenantId, searchTerm, status, pageNumber, pageSize);

                var loanDtos = _mapper.Map<List<Loansdto>>(loans);

                var response = new PagedResponse<Loansdto>(loanDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} loans out of {TotalCount} for tenant {TenantId}",
                    loanDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching loans for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get loan by ID
        /// </summary>
        /// <param name="id">Loan ID</param>
        /// <returns>Loan details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Loansdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Loansdto>>> GetLoansById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching loans with ID: {LoanId}", id);

                var loan = await _unitOfWork.Loans.GetByIdAsync(id);

                if (loan == null)
                {
                    _logger.LogWarning("Loan with ID {LoanId} not found", id);
                    return NotFound(new ApiResponse<object>($"LOan with ID {id} not found"));
                }

                var loanDto = _mapper.Map<Loansdto>(loan);

                _logger.LogInformation("Successfully retrieved loan {LoanId}", id);

                return Ok(new ApiResponse<Loansdto>(loanDto, "Loan retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching Loans {LoanId}", id);
                throw;
            }
        }

        /// <summary>
        /// Create a new loan
        /// </summary>
        /// <param name="createLoansdto">Cards creation data</param>
        /// <returns>Created loan details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Loansdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Loansdto>>> CreateLoans([FromBody] CreateLoansdto createLoansdto)
        {
            try
            {
                _logger.LogInformation("Creating new loan  tenant {TenantId}",
                     createLoansdto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createLoansdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for loan creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }



                // Map DTO to entity
                var loan = _mapper.Map<Loans>(createLoansdto);
                loan.LoanId = Guid.NewGuid();
                loan.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.Loans.AddAsync(loan);
                await _unitOfWork.SaveChangesAsync();

                var loanDto = _mapper.Map<Loansdto>(loan);

                _logger.LogInformation("Successfully created loan loanId",
                    loan.LoanId);

                return CreatedAtAction(
                    nameof(GetLoansById),
                    new { id = loan.LoanId },
                    new ApiResponse<Loansdto>(loanDto, "Card created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating loan");
                throw;
            }
        }

        /// <summary>
        /// Update an existing loan
        /// </summary>
        /// <param name="id">Loan ID</param>
        /// <param name="updateLoansdto">Loan update data</param>
        /// <returns>Updated loans details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Loansdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Loansdto>>> UpdateLoans(
            Guid id,
            [FromBody] UpdateLoansdto updateLoansdto)
        {
            try
            {
                _logger.LogInformation("Updating loan {LoanId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateLoansdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for laon update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing card
                var existingLoan = await _unitOfWork.Loans.GetByIdAsync(id);
                if (existingLoan == null)
                {
                    _logger.LogWarning("Loan {LoanId} not found", id);
                    return NotFound(new ApiResponse<object>($"Loan with ID {id} not found"));
                }

                // Update only provided fields


                if (!string.IsNullOrWhiteSpace(updateLoansdto.LoanStatus))
                    existingLoan.LoanStatus = updateLoansdto.LoanStatus;

                existingLoan.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Loans.Update(existingLoan);
                await _unitOfWork.SaveChangesAsync();

                var loanDto = _mapper.Map<Loansdto>(existingLoan);

                _logger.LogInformation("Successfully updated loan {LoanId}", id);

                return Ok(new ApiResponse<Loansdto>(loanDto, "Loan updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating loan {LoanId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a loan (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Loan ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteLoans(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting Loans {LoanId}", id);

                var loan = await _unitOfWork.Loans.GetByIdAsync(id);
                if (loan == null)
                {
                    _logger.LogWarning("Loans {LoanId} not found", id);
                    return NotFound(new ApiResponse<object>($"Loan with ID {id} not found"));
                }

                // Soft delete
                loan.LoanStatus = "Inactive";
                loan.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Loans.Update(loan);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted loan {LoanId}", id);

                return Ok(new ApiResponse<object>(null, "Loan deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting loan {LoanId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search loans by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching loans</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<Loansdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Loansdto>>>> SearchCards(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching loans for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var loan = await _unitOfWork.Loans.SearchLoansAsync(tenantId, searchTerm);
                var loanDtos = _mapper.Map<List<Loansdto>>(loan);

                _logger.LogInformation("Found {Count} loans matching search term", loanDtos.Count);

                return Ok(new ApiResponse<List<Loansdto>>(loanDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching loans");
                throw;
            }
        }

        /// <summary>
        /// Get active loans for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active laonss</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<Loansdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Loansdto>>>> GetActiveLoans([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active loans for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var loan = await _unitOfWork.Loans.GetActiveLoansAsync(tenantId);
                var loanDtos = _mapper.Map<List<Loansdto>>(loan);

                _logger.LogInformation("Successfully retrieved {Count} active loans", loanDtos.Count);

                return Ok(new ApiResponse<List<Loansdto>>(loanDtos, "Active loans retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active loans for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}