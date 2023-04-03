using FusionPlannerAPI.Boundary.Request;
using FusionPlannerAPI.Boundary.Response;
using FusionPlannerAPI.Exceptions;
using FusionPlannerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FusionPlannerAPI.Controllers
{
    [ApiController]
    [Route("api/boards")]
    public class BoardController : ControllerBase
    {
        private readonly IBoardService _boardService;

        public BoardController(IBoardService boardService)
        {
            _boardService = boardService;
        }

        [HttpGet]
        [Route("{boardId}")]
        public async Task<IActionResult> Get([FromRoute] int boardId)
        {
            var board = await _boardService.GetById(boardId);
            if (board == null) return NotFound(boardId);

            return Ok(board);
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var boards = await _boardService.ListBoards();

            return Ok(boards);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBoardRequest request)
        {
            var userId = 1;

            try
            {
                var boardId = await _boardService.CreateBoard(request, userId);

                return Created(nameof(Get), new { id = boardId });
            }
            catch (UserNotFoundException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("{boardId}")]
        public async Task<IActionResult> Edit([FromRoute] int boardId, [FromBody]  EditBoardRequest request)
        {
            try
            {
                await _boardService.EditBoard(boardId, request);

                return NoContent();
            }
            catch (BoardNotFoundException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Route("{boardId}")]
        public async Task<IActionResult> Delete([FromRoute] int boardId)
        {
            try
            {
                await _boardService.DeleteBoard(boardId);

                return NoContent();
            }
            catch (BoardNotFoundException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
