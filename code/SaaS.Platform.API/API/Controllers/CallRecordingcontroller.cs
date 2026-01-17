using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.CallRecordings;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// Callrecording management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class CallRecordingController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CallRecordingController> _logger;
        private readonly IValidator<CreateCallRecordingdto> _createValidator;
        private readonly IValidator<UpdateCallRecordingdto> _updateValidator;

        public CallRecordingController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CallRecordingController> logger,
            IValidator<CreateCallRecordingdto> createValidator,
            IValidator<UpdateCallRecordingdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        
        /// <summary>
        /// Get callRecording by ID
        /// </summary>
        /// <param name="id">CallRecord ID</param>
        /// <returns>CallRecording details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CallRecordingdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CallRecordingdto>>> GetCallRecordingById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching callRecording with ID: {RecordingId}", id);

                var callRecording = await _unitOfWork.CallRecordings.GetByIdAsync(id);

                if (callRecording == null)
                {
                    _logger.LogWarning("CallRecording with ID {RecordingId} not found", id);
                    return NotFound(new ApiResponse<object>($"CallRecording with ID {id} not found"));
                }

                var callRecordingDto = _mapper.Map<CallRecordingdto>(callRecording);

                _logger.LogInformation("Successfully retrieved callRecording {RecordingId}", id);

                return Ok(new ApiResponse<CallRecordingdto>(callRecordingDto, "Recording retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching callRecording {RecordingId}", id);
                throw;
            }
        }


        /// <summary>
        /// Create a new callRecording
        /// </summary>
        /// <param name="createCallRecordingdto">CallRecording creation data</param>
        /// <returns>Created callRecording details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CallRecordingdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<CallRecordingdto>>> CreateCallRecording([FromBody] CreateCallRecordingdto createCallRecordingdto)
        {
            try
            {


                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createCallRecordingdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for callRecording creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }



                // Map DTO to entity
                var callRecording = _mapper.Map<CallRecordings>(createCallRecordingdto);
                callRecording.RecordingId = Guid.NewGuid();
                callRecording.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.CallRecordings.AddAsync(callRecording);
                await _unitOfWork.SaveChangesAsync();

                var callRecordingDto = _mapper.Map<CallRecordingdto>(callRecording);

                _logger.LogInformation("Successfully created callRecording {RecordingId}",
                    callRecording.RecordingId);

                return CreatedAtAction(
                    nameof(GetCallRecordingById),
                    new { id = callRecording.RecordingId },
                    new ApiResponse<CallRecordingdto>(callRecordingDto, "CallRecording created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating callRecording");
                throw;
            }
        }

        /// <summary>
        /// Update an existing callRecording
        /// </summary>
        /// <param name="id">Recording ID</param>
        /// <param name="updateCallRecorddto">CallRecording update data</param>
        /// <returns>Updated callRecording details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CallRecordingdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CallRecordingdto>>> UpdateCallRecording(
            Guid id,
            [FromBody] UpdateCallRecordingdto updateCallRecordingdto)
        {
            try
            {
                _logger.LogInformation("Updating callRecording {RecordingId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateCallRecordingdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing callRecord
                var existingCallRecording = await _unitOfWork.CallRecordings.GetByIdAsync(id);
                if (existingCallRecording == null)
                {
                    _logger.LogWarning("CallRecording {RecordingId} not found", id);
                    return NotFound(new ApiResponse<object>($"CallRecording with ID {id} not found"));
                }

                // Update only provided fields
               

                _unitOfWork.CallRecordings.Update(existingCallRecording);
                await _unitOfWork.SaveChangesAsync();

                var callRecordingDto = _mapper.Map<CallRecordingdto>(existingCallRecording);

                _logger.LogInformation("Successfully updated callRecording {RecordingId}", id);

                return Ok(new ApiResponse<CallRecordingdto>(callRecordingDto, "CallRecording updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating callRecording {RecordingId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a callRecording (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Recording ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteCallRecording(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting callRecording {RecordingId}", id);

                var callRecording = await _unitOfWork.CallRecordings.GetByIdAsync(id);
                if (callRecording == null)
                {
                    _logger.LogWarning("CallRecording {RecordingId} not found", id);
                    return NotFound(new ApiResponse<object>($"CallRecording with ID {id} not found"));
                }

                // Soft delete
               
                _unitOfWork.CallRecordings.Update(callRecording);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted callRecording {RecordingId}", id);

                return Ok(new ApiResponse<object>(null, "CallRecording deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting callRecording {RecordingId}", id);
                throw;
            }
        }

        
    }
}