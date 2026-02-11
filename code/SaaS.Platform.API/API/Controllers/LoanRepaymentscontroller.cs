using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.LoanRepayments;
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
    public class LoanRepaymentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<LoanRepaymentsController> _logger;
        private readonly IValidator<CreateLoanRepaymentsdto> _createValidator;
        private readonly IValidator<UpdateLoanRepaymentsdto> _updateValidator;

        public LoanRepaymentsController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<LoanRepaymentsController> logger,
            IValidator<CreateLoanRepaymentsdto> createValidator,
            IValidator<UpdateLoanRepaymentsdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        

        /// <summary>
        /// Get loan by ID
        /// </summary>
        /// <param name="id">Loan ID</param>
        /// <returns>LoanRepayment details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<LoanRepaymentsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<LoanRepaymentsdto>>> GetLoanById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching loans with ID: {LoanId}", id);

                var loanRepayment = await _unitOfWork.LoanRepayments.GetByIdAsync(id);

                if (loanRepayment == null)
                {
                    _logger.LogWarning("Loan with ID {LoanId} not found", id);
                    return NotFound(new ApiResponse<object>($"Loan with ID {id} not found"));
                }

                var loanRepaymentDto = _mapper.Map<LoanRepaymentsdto>(loanRepayment);

                _logger.LogInformation("Successfully retrieved loanRepayment {LoanId}", id);

                return Ok(new ApiResponse<LoanRepaymentsdto>(loanRepaymentDto, "LoanRepayment retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching LoanRepayments {LoanId}", id);
                throw;
            }
        }
    }
}
