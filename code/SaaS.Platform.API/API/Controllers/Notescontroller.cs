using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Notes;
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
    public class NotesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<NotesController> _logger;
        private readonly IValidator<CreateNotedto> _createValidator;
        private readonly IValidator<UpdateNotedto> _updateValidator;

        public NotesController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<NotesController> logger,
            IValidator<CreateNotedto> createValidator,
            IValidator<UpdateNotedto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }
        /// <summary>
        /// Get all notes with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of notes</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<Notedto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<Notedto>>> GetNotes(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching notes for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (notes, totalCount) = await _unitOfWork.Note.GetNotesPagedAsync(
                    tenantId, searchTerm, pageNumber, pageSize);

                var noteDtos = _mapper.Map<List<Notedto>>(notes);

                var response = new PagedResponse<Notedto>(noteDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} notes out of {TotalCount} for tenant {TenantId}",
                    noteDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching notes for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get notes by ID
        /// </summary>
        /// <param name="id">Notes ID</param>
        /// <returns>Notes details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Notedto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Notedto>>> GetNoteById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching note with ID: {NoteId}", id);

                var note = await _unitOfWork.Note.GetByIdAsync(id);

                if (note == null)
                {
                    _logger.LogWarning("Note with ID {NoteId} not found", id);
                    return NotFound(new ApiResponse<object>($"Note with ID {id} not found"));
                }

                var noteDto = _mapper.Map<Notedto>(note);

                _logger.LogInformation("Successfully retrieved note {NoteId}", id);

                return Ok(new ApiResponse<Notedto>(noteDto, "Note retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching note {NoteId}", id);
                throw;
            }
        }

        /// <summary>
        /// Create a new note
        /// </summary>
        /// <param name="createNotedto">Note creation data</param>
        /// <returns>Created note details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Notedto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Notedto>>> CreateNote([FromBody] CreateNotedto createNotedto)
        {
            try
            {
                _logger.LogInformation("Creating new note  for tenant {TenantId}",
                     createNotedto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createNotedto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for note creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

               

                // Map DTO to entity
                var note = _mapper.Map<Notes>(createNotedto);
                note.NoteId = Guid.NewGuid();
                note.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.Note.AddAsync(note);
                await _unitOfWork.SaveChangesAsync();

                var noteDto = _mapper.Map<Notedto>(note);

                _logger.LogInformation("Successfully created note {NoteId} }",
                    note.NoteId);

                return CreatedAtAction(
                    nameof(GetNoteById),
                    new { id = note.NoteId },
                    new ApiResponse<Notedto>(noteDto, "Note created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating note");
                throw;
            }
        }

        /// <summary>
        /// Update an existing note
        /// </summary>
        /// <param name="id">Notes ID</param>
        /// <param name="updateNotedto">Note update data</param>
        /// <returns>Updated note details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Notedto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Notedto>>> UpdateNote(
            Guid id,
            [FromBody] UpdateNotedto updateNotedto)
        {
            try
            {
                _logger.LogInformation("Updating note {NoteId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateNotedto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for note update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing note
                var existingNote = await _unitOfWork.Note.GetByIdAsync(id);
                if (existingNote == null)
                {
                    _logger.LogWarning("Note {NoteId} not found", id);
                    return NotFound(new ApiResponse<object>($"Note with ID {id} not found"));
                }

                // Update only provided fields
               
                 if (updateNotedto.IsPrivate.HasValue)
                    existingNote.IsPrivate = updateNotedto.IsPrivate.Value;

                existingNote.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Note.Update(existingNote);
                await _unitOfWork.SaveChangesAsync();

                var noteDto = _mapper.Map<Notedto>(existingNote);

                _logger.LogInformation("Successfully updated note {NoteId}", id);

                return Ok(new ApiResponse<Notedto>(noteDto, "Note updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating note {NoteId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a note (soft delete by setting IsPrivate to false)
        /// </summary>
        /// <param name="id">Note ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteNote(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting note {NoteId}", id);

                var note = await _unitOfWork.Note.GetByIdAsync(id);
                if (note == null)
                {
                    _logger.LogWarning("Note {NoteId} not found", id);
                    return NotFound(new ApiResponse<object>($"Note with ID {id} not found"));
                }

                // Soft delete
                note.IsPrivate = false;
                note.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Note.Update(note);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted note {NoteId}", id);

                return Ok(new ApiResponse<object>(null, "Note deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting note {NoteId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search notes by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching notes</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<Notedto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Notedto>>>> SearchNotes(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching notes for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var notes = await _unitOfWork.Note.SearchNotesAsync(tenantId, searchTerm);
                var noteDtos = _mapper.Map<List<Notedto>>(notes);

                _logger.LogInformation("Found {Count} notes matching search term", noteDtos.Count);

                return Ok(new ApiResponse<List<Notedto>>(noteDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching notes");
                throw;
            }
        }

        /// <summary>
        /// Get active notes for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active notes</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<Notedto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Notedto>>>> GetActiveNotes([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active note for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var notes = await _unitOfWork.Note.GetActiveNotesAsync(tenantId);
                var noteDtos = _mapper.Map<List<Notedto>>(notes);

                _logger.LogInformation("Successfully retrieved {Count} active notes", noteDtos.Count);

                return Ok(new ApiResponse<List<Notedto>>(noteDtos, "Active notes retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active notes for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}
