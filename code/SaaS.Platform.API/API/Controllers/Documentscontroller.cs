using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Documents;
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
    public class DocumentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DocumentsController> _logger;
        private readonly IValidator<CreateDocumentsdto> _createValidator;
        private readonly IValidator<UpdateDocumentsdto> _updateValidator;

        public DocumentsController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<DocumentsController> logger,
            IValidator<CreateDocumentsdto> createValidator,
            IValidator<UpdateDocumentsdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all documents with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of customers</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<Documentsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<Documentsdto>>> GetDocuments(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching documents for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (documents, totalCount) = await _unitOfWork.Documents.GetDocumentsPagedAsync(
                    tenantId, searchTerm,  pageNumber, pageSize);

                var documentDtos = _mapper.Map<List<Documentsdto>>(documents);

                var response = new PagedResponse<Documentsdto>(documentDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} documents out of {TotalCount} for tenant {TenantId}",
                    documentDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching customers for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get document by ID
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <returns>Document details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Documentsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Documentsdto>>> GetDocumentsById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching document with ID: {DocumentId}", id);

                var document = await _unitOfWork.Documents.GetByIdAsync(id);

                if (document == null)
                {
                    _logger.LogWarning("Document with ID {DocumentId} not found", id);
                    return NotFound(new ApiResponse<object>($"Document with ID {id} not found"));
                }

                var documentDto = _mapper.Map<Documentsdto>(document);

                _logger.LogInformation("Successfully retrieved document {DocumentId}", id);

                return Ok(new ApiResponse<Documentsdto>(documentDto, "Document retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching document {DocumentId}", id);
                throw;
            }
        }

       
        /// <summary>
        /// Create a new document
        /// </summary>
        /// <param name="createDocumentsdto">Customer creation data</param>
        /// <returns>Created document details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Documentsdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Documentsdto>>> CreateDocuments([FromBody] CreateDocumentsdto createDocumentsdto)
        {
            try
            {
                _logger.LogInformation("Creating new document  for tenant {TenantId}",
                     createDocumentsdto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createDocumentsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                

                // Map DTO to entity
                var document = _mapper.Map<Documents>(createDocumentsdto);
                document.DocumentId = Guid.NewGuid();
                document.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.Documents.AddAsync(document);
                await _unitOfWork.SaveChangesAsync();

                var documentDto = _mapper.Map<Documentsdto>(document);

                _logger.LogInformation("Successfully created document {DocumentId}",
                    document.DocumentId);

                return CreatedAtAction(
                    nameof(GetDocumentsById),
                    new { id = document.DocumentId },
                    new ApiResponse<Documentsdto>(documentDto, "Document created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating document");
                throw;
            }
        }

        /// <summary>
        /// Update an existing document
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <param name="updateDocumentsdto">Document update data</param>
        /// <returns>Updated document details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Documentsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Documentsdto>>> UpdateDocuments(
            Guid id,
            [FromBody] UpdateDocumentsdto updateDocumentsdto)
        {
            try
            {
                _logger.LogInformation("Updating document {DocumentId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateDocumentsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for customer update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing document
                var existingDocument = await _unitOfWork.Documents.GetByIdAsync(id);
                if (existingDocument == null)
                {
                    _logger.LogWarning("Document {DocumentId} not found", id);
                    return NotFound(new ApiResponse<object>($"Document with ID {id} not found"));
                }

                // Update only provided fields
               

                if (!string.IsNullOrWhiteSpace(updateDocumentsdto.DocumentName))
                    existingDocument.DocumentName = updateDocumentsdto.DocumentName;

                
                _unitOfWork.Documents.Update(existingDocument);
                await _unitOfWork.SaveChangesAsync();

                var documentDto = _mapper.Map<Documentsdto>(existingDocument);

                _logger.LogInformation("Successfully updated document {DocumentId}", id);

                return Ok(new ApiResponse<Documentsdto>(documentDto, "Document updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating document {DocumentId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a document (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteDocuments(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting document {DocumentId}", id);

                var document = await _unitOfWork.Documents.GetByIdAsync(id);
                if (document == null)
                {
                    _logger.LogWarning("Document {DocumentId} not found", id);
                    return NotFound(new ApiResponse<object>($"Document with ID {id} not found"));
                }

                // Soft delete
               

                _unitOfWork.Documents.Update(document);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted document {DocumentId}", id);

                return Ok(new ApiResponse<object>(null, "Document deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting document {DocumentId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search document by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching document</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<Documentsdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Documentsdto>>>> SearchDocuments(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching documents for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var document = await _unitOfWork.Documents.SearchDocumentsAsync(tenantId, searchTerm);
                var documentDtos = _mapper.Map<List<Documentsdto>>(document);

                _logger.LogInformation("Found {Count} document matching search term", documentDtos.Count);

                return Ok(new ApiResponse<List<Documentsdto>>(documentDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching document");
                throw;
            }
        }

        /// <summary>
        /// Get active documents for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active documents</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<Documentsdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Documentsdto>>>> GetActiveDocuments([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active documents for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var document = await _unitOfWork.Documents.GetActiveDocumentsAsync(tenantId);
                var documentDtos = _mapper.Map<List<Documentsdto>>(document);

                _logger.LogInformation("Successfully retrieved {Count} active documents", documentDtos.Count);

                return Ok(new ApiResponse<List<Documentsdto>>(documentDtos, "Active documents retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active documents for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}