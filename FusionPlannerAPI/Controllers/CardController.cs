using FusionPlannerAPI.Boundary.Request;
using FusionPlannerAPI.Exceptions;
using FusionPlannerAPI.Infrastructure;
using FusionPlannerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace FusionPlannerAPI.Controllers
{
    [ApiController]
    [Route("api/cards")]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;

        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [HttpGet]
        [Route("{cardId}")]
        public async Task<IActionResult> Get([FromRoute] int cardId)
        {
            var card = await _cardService.GetById(cardId);
            if (card == null) return NotFound(cardId);

            return Ok(card);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCard([FromBody] CreateCardRequest request)
        {
            var userId = 1;

            try
            {
                var cardId = await _cardService.CreateCard(request, userId);

                return Created(nameof(Get), new { id = cardId });
            }
            catch (ColumnNotFoundException e)
            {
                return BadRequest(e.Message);
            }
            catch (UserNotFoundException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("{cardId}")]
        public async Task<IActionResult> EditCard([FromRoute] int cardId, [FromBody] EditCardRequest request)
        {
            try
            {
                await _cardService.EditCard(cardId, request);

                return NoContent();
            }
            catch (CardNotFoundException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Route("{cardId}")]
        public async Task<IActionResult> Delete([FromRoute] int cardId)
        {
            try
            {
                await _cardService.DeleteCard(cardId);

                return NoContent();
            }
            catch (CardNotFoundException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("{cardId}/archive")]
        public async Task<IActionResult> Archive([FromRoute] int cardId)
        {
            try
            {
                await _cardService.ArchiveCard(cardId);

                return NoContent();
            }
            catch (CardNotFoundException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
