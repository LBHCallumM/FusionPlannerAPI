using FusionPlannerAPI.Boundary.Request;
using FusionPlannerAPI.Boundary.Response;
using FusionPlannerAPI.Gateways.Interfaces;
using FusionPlannerAPI.Infrastructure;
using FusionPlannerAPI.Services.Interfaces;

namespace FusionPlannerAPI.Services
{
    public class BoardService : IBoardService
    {
        private readonly IBoardGateway _gateway;

        public BoardService(IBoardGateway boardGateway)
        {
            _gateway = boardGateway;
        }

        public async Task<BoardResponse> GetById(int boardId)
        {
            return await _gateway.GetById(boardId);
        }

        public async Task<int> CreateBoard(CreateBoardRequest request, int userId)
        {
            return await _gateway.CreateBoard(request, userId);
        }

        public async Task EditBoard(int boardId, EditBoardRequest request)
        {
            await _gateway.EditBoard(boardId, request);
        }

        public async Task DeleteBoard(int boardId)
        {
            await _gateway.DeleteBoard(boardId);
        }

        public async Task<IEnumerable<BoardResponse>> ListBoards()
        {
            return await _gateway.ListBoards();
        }
    }
}
