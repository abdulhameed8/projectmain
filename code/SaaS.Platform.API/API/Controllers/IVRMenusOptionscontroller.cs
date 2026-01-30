using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.IVRMenusOptions;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// IVRMenusOptions management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class IVRMenusOptionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<IVRMenusOptionsController> _logger;
        private readonly IValidator<CreateIVRMenusOptionsdto> _createValidator;
        private readonly IValidator<UpdateIVRMenusOptionsdto> _updateValidator;

        public IVRMenusOptionsController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<IVRMenusOptionsController> logger,
            IValidator<CreateIVRMenusOptionsdto> createValidator,
            IValidator<UpdateIVRMenusOptionsdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        
        /// <summary>
        /// Get MenuOptions by ID
        /// </summary>
        /// <param name="id">MenuOption ID</param>
        /// <returns>MenuOption details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<IVRMenusOptionsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<IVRMenusOptionsdto>>> GetMenuOptionsById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching IVRMenuOption with ID: {MenuOptionId}", id);

                var iVRMenusOption = await _unitOfWork.IVRMenusOptions.GetByIdAsync(id);

                if (iVRMenusOption == null)
                {
                    _logger.LogWarning("MenuOption with ID {MenuOptionId} not found", id);
                    return NotFound(new ApiResponse<object>($"IVRMenusOptions with ID {id} not found"));
                }

                var iVRMenusOptionsDto = _mapper.Map<IVRMenusOptionsdto>(iVRMenusOption);

                _logger.LogInformation("Successfully retrieved iVRMenusOption {MenuOptionId}", id);

                return Ok(new ApiResponse<IVRMenusOptionsdto>(iVRMenusOptionsDto, "IVRMenusOptions retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching iVRMenuOption {MenuOptionId}", id);
                throw;
            }
        }

        
        
        /// <summary>
        /// Update an existing customer
        /// </summary>
        /// <param name="id">MenuOption ID</param>
        /// <param name="updateMenuOptionsdto">MenuOptions update data</param>
        /// <returns>Updated IVRMenusOptions details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<IVRMenusOptionsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<IVRMenusOptionsdto>>> UpdateIVRMenusOptions(
            Guid id,
            [FromBody] UpdateIVRMenusOptionsdto updateIVRMenusOptionsdto)
        {
            try
            {
                _logger.LogInformation("Updating iVRMenuoption {MenuOptionId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateIVRMenusOptionsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing iVRMenusOption
                var existingIVRMenusOptions = await _unitOfWork.IVRMenusOptions.GetByIdAsync(id);
                if (existingIVRMenusOptions == null)
                {
                    _logger.LogWarning("IVRMenusOption {MenuOptionId} not found", id);
                    return NotFound(new ApiResponse<object>($"IVRMenusoption with ID {id} not found"));
                }

                // Update only provided fields
                

                if (updateIVRMenusOptionsdto.IsActive.HasValue)
                    existingIVRMenusOptions.IsActive = updateIVRMenusOptionsdto.IsActive.Value;


                _unitOfWork.IVRMenusOptions.Update(existingIVRMenusOptions);
                await _unitOfWork.SaveChangesAsync();

                var iVRMenusOptionDto = _mapper.Map<IVRMenusOptionsdto>(existingIVRMenusOptions);

                _logger.LogInformation("Successfully updated iVRMenusoptions {MenuOptionId}", id);

                return Ok(new ApiResponse<IVRMenusOptionsdto>(iVRMenusOptionDto, "IVRMenusOption updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating iVRMenusOptions {MenuOptionId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a iVRMenusOption (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">MenuOption ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteIVRMenuOptions(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting iVRMenusOption {MenuOptionId}", id);

                var iVRMenusOption = await _unitOfWork.IVRMenusOptions.GetByIdAsync(id);
                if (iVRMenusOption == null)
                {
                    _logger.LogWarning("IVRMenusOption {MenuOptionId} not found", id);
                    return NotFound(new ApiResponse<object>($"MenuOption with ID {id} not found"));
                }

                // Soft delete
                iVRMenusOption.IsActive = false;

                _unitOfWork.IVRMenusOptions.Update(iVRMenusOption);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted iVRMenusOption {MenuOptionId}", id);

                return Ok(new ApiResponse<object>(null, "IVRMenusOption deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting iVRMenusOption {MenuOptionId}", id);
                throw;
            }
        }

        
    }
}
    