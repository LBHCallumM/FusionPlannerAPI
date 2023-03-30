using AutoFixture;
using FusionPlannerAPI.Boundary.Request;
using FusionPlannerAPI.Boundary.Response;
using FusionPlannerAPI.Exceptions;
using FusionPlannerAPI.Factories;
using FusionPlannerAPI.Gateways.Interfaces;
using FusionPlannerAPI.Infrastructure;
using FusionPlannerAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FusionPlannerAPI.Gateways
{
    public class BoardGateway : IBoardGateway
    {
        private readonly PlannerDbContext _context;

        public BoardGateway(PlannerDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateBoard(CreateBoardRequest request, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new UserNotFoundException(userId);

            var board = request.ToDatabase(userId);

            _context.Boards.Add(board);
            await _context.SaveChangesAsync();

            return board.Id;
        }

        public async Task EditBoard(int boardId, EditBoardRequest request)
        {
            var board = await _context.Boards.FindAsync(boardId);
            if (board == null) throw new BoardNotFoundException(boardId);

            board.Name = request.Name;
            board.Description = request.Description;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteBoard(int boardId)
        {
            var board = await _context.Boards.FindAsync(boardId);
            if (board == null) throw new BoardNotFoundException(boardId);

            // ToDo - Check for columns/cards

            _context.Boards.Remove(board);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BoardResponse>> ListBoards()
        {
            var boards = await _context.Boards.ToListAsync();

            return boards.ToResponse();
        }

        public async Task<BoardResponse> GetById(int boardId)
        {
            var board = await _context.Boards.FindAsync(boardId);
            if (board == null) return null;

            return board.ToResponse();

        }
    }
}
