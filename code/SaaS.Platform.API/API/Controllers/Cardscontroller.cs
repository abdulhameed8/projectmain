using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaaS.Platform.API.Application.Common;
using SaaS.Platform.API.Application.DTOs.Cards;
using SaaS.Platform.API.Domain.Entities;
using SaaS.Platform.API.Infrastructure.UnitOfWork;

namespace SaaS.Platform.API.API.Controllers
{
    /// <summary>
    /// Cards management API controller
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class CardsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CardsController> _logger;
        private readonly IValidator<CreateCardsdto> _createValidator;
        private readonly IValidator<UpdateCardsdto> _updateValidator;

        public CardsController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CardsController> logger,
            IValidator<CreateCardsdto> createValidator,
            IValidator<UpdateCardsdto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        /// <summary>
        /// Get all cards with optional pagination and filtering
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term for filtering</param>
        /// <param name="status">Customer status filter</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paginated list of customers</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<Cardsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<Cardsdto>>> GetCards(
            [FromQuery] Guid tenantId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? status = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Fetching cards for tenant {TenantId} - Page: {PageNumber}, Size: {PageSize}",
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

                var (cards, totalCount) = await _unitOfWork.Cards.GetCardsPagedAsync(
                    tenantId, searchTerm, status, pageNumber, pageSize);

                var cardDtos = _mapper.Map<List<Cardsdto>>(cards);

                var response = new PagedResponse<Cardsdto>(cardDtos, pageNumber, pageSize, totalCount);

                _logger.LogInformation("Successfully retrieved {Count} cards out of {TotalCount} for tenant {TenantId}",
                    cardDtos.Count, totalCount, tenantId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching cards for tenant {TenantId}", tenantId);
                throw;
            }
        }

        /// <summary>
        /// Get card by ID
        /// </summary>
        /// <param name="id">Card ID</param>
        /// <returns>Card details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Cardsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Cardsdto>>> GetCardsById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching cards with ID: {CardId}", id);

                var card = await _unitOfWork.Cards.GetByIdAsync(id);

                if (card == null)
                {
                    _logger.LogWarning("Card with ID {CardId} not found", id);
                    return NotFound(new ApiResponse<object>($"Card with ID {id} not found"));
                }

                var cardDto = _mapper.Map<Cardsdto>(card);

                _logger.LogInformation("Successfully retrieved card {Cardd}", id);

                return Ok(new ApiResponse<Cardsdto>(cardDto, "Card retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching card {CardId}", id);
                throw;
            }
        }

        
        /// <summary>
        /// Create a new card
        /// </summary>
        /// <param name="createCardsdto">Cards creation data</param>
        /// <returns>Created card details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Cardsdto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Cardsdto>>> CreateCards([FromBody] CreateCardsdto createCardsdto)
        {
            try
            {
                _logger.LogInformation("Creating new card  tenant {TenantId}",
                     createCardsdto.TenantId);

                // Validate input
                var validationResult = await _createValidator.ValidateAsync(createCardsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for card creation: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                

                // Map DTO to entity
                var card = _mapper.Map<Cards>(createCardsdto);
                card.CardId = Guid.NewGuid();
                card.CreatedDate = DateTime.UtcNow;

                // Add to database
                await _unitOfWork.Cards.AddAsync(card);
                await _unitOfWork.SaveChangesAsync();

                var cardDto = _mapper.Map<Cardsdto>(card);

                _logger.LogInformation("Successfully created card CardId",
                    card.CardId);

                return CreatedAtAction(
                    nameof(GetCardsById),
                    new { id = card.CardId },
                    new ApiResponse<Cardsdto>(cardDto, "Card created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating card");
                throw;
            }
        }

        /// <summary>
        /// Update an existing card
        /// </summary>
        /// <param name="id">Card ID</param>
        /// <param name="updateCardsdto">Card update data</param>
        /// <returns>Updated cards details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Cardsdto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<Cardsdto>>> UpdateCards(
            Guid id,
            [FromBody] UpdateCardsdto updateCardsdto)
        {
            try
            {
                _logger.LogInformation("Updating card {CardId}", id);

                // Validate input
                var validationResult = await _updateValidator.ValidateAsync(updateCardsdto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Validation failed for card update: {Errors}",
                        string.Join(", ", errors));
                    return BadRequest(new ApiResponse<object>("Validation failed", errors));
                }

                // Get existing card
                var existingCard = await _unitOfWork.Cards.GetByIdAsync(id);
                if (existingCard == null)
                {
                    _logger.LogWarning("Card {CardId} not found", id);
                    return NotFound(new ApiResponse<object>($"Card with ID {id} not found"));
                }

                // Update only provided fields
                
                
               if (!string.IsNullOrWhiteSpace(updateCardsdto.CardStatus))
                    existingCard.CardStatus = updateCardsdto.CardStatus;

                existingCard.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Cards.Update(existingCard);
                await _unitOfWork.SaveChangesAsync();

                var cardDto = _mapper.Map<Cardsdto>(existingCard);

                _logger.LogInformation("Successfully updated card {CardId}", id);

                return Ok(new ApiResponse<Cardsdto>(cardDto, "Card updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating card {CardId}", id);
                throw;
            }
        }

        /// <summary>
        /// Delete a card (soft delete by setting IsActive to false)
        /// </summary>
        /// <param name="id">Card ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteCards(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting cards {CardId}", id);

                var card = await _unitOfWork.Cards.GetByIdAsync(id);
                if (card == null)
                {
                    _logger.LogWarning("Cards {CardId} not found", id);
                    return NotFound(new ApiResponse<object>($"Card with ID {id} not found"));
                }

                // Soft delete
                card.CardStatus = "Inactive";
                card.ModifiedDate = DateTime.UtcNow;

                _unitOfWork.Cards.Update(card);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted card {CardId}", id);

                return Ok(new ApiResponse<object>(null, "Card deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting card {CardId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search cards by search term
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching cards</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<Cardsdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Cardsdto>>>> SearchCards(
            [FromQuery] Guid tenantId,
            [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching cards for tenant {TenantId} with term: {SearchTerm}",
                    tenantId, searchTerm);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new ApiResponse<object>("Search term is required"));
                }

                var cards = await _unitOfWork.Cards.SearchCardsAsync(tenantId, searchTerm);
                var cardDtos = _mapper.Map<List<Cardsdto>>(cards);

                _logger.LogInformation("Found {Count} cards matching search term", cardDtos.Count);

                return Ok(new ApiResponse<List<Cardsdto>>(cardDtos, "Search completed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching cards");
                throw;
            }
        }

        /// <summary>
        /// Get active cards for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of active cards</returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ApiResponse<List<Cardsdto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<List<Cardsdto>>>> GetActiveCards([FromQuery] Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Fetching active customers for tenant {TenantId}", tenantId);

                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new ApiResponse<object>("Tenant ID is required"));
                }

                var cards = await _unitOfWork.Cards.GetActiveCardsAsync(tenantId);
                var cardDtos = _mapper.Map<List<Cardsdto>>(cards);

                _logger.LogInformation("Successfully retrieved {Count} active cards", cardDtos.Count);

                return Ok(new ApiResponse<List<Cardsdto>>(cardDtos, "Active cards retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active cards for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}