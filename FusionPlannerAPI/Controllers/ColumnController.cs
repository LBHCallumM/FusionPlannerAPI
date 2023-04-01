using FusionPlannerAPI.Boundary.Request;
using FusionPlannerAPI.Exceptions;
using FusionPlannerAPI.Infrastructure;
using FusionPlannerAPI.Services;
using FusionPlannerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FusionPlannerAPI.Controllers
{
    [ApiController]
    [Route("api/columns")]
    public class ColumnController : ControllerBase
    {
        private readonly IColumnService _columnService;

        public ColumnController(IColumnService columnService)
        {
            _columnService = columnService;
        }

        [HttpGet("{columId}")]
        public async Task<IActionResult> Get([FromRoute] int columnId)
        {
            var column = await _columnService.GetById(columnId);
            if (column == null) return NotFound(columnId);

            return Ok(column);
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery(Name = "BoardId")] int boardId)
        {
            try
            {
                var columns = await _columnService.ListColumns(boardId);

                return Ok(columns);
            }
            catch (BoardNotFoundException e)
            {
                return BadRequest($"bOARD not found with id {e.BoardId}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateColumnRequest request)
        {
            try
            {
                var columnId = await _columnService.CreateColumn(request);

                return Created(nameof(Get), new { id = columnId });
            }
            catch (BoardNotFoundException e)
            {
                return BadRequest($"Board not found with id {e.BoardId}");
            }
        }

        [HttpPut]
        [Route("{columnId}")]
        public async Task<IActionResult> Edit([FromRoute] int columnId, [FromBody] EditColumnRequest request)
        {
            try
            {
                await _columnService.EditColumn(columnId, request);

                return NoContent();
            }
            catch (ColumnNotFoundException e)
            {
                return BadRequest($"Column not found with id {e.ColumnId}");
            }
        }

        [HttpPost]
        [Route("{columnId}")]
        public async Task<IActionResult> Delete([FromRoute] int columnId)
        {
            try
            {
                await _columnService.DeleteColumn(columnId);

                return NoContent();
            }
            catch (ColumnNotFoundException e)
            {
                return BadRequest($"Column not found with id {e.ColumnId}");
            }
        }
    }
}
