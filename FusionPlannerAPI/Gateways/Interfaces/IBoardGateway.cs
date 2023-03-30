using FusionPlannerAPI.Boundary.Request;
using FusionPlannerAPI.Boundary.Response;
using FusionPlannerAPI.Infrastructure;

namespace FusionPlannerAPI.Gateways.Interfaces
{
    public interface IBoardGateway
    {
        Task<BoardResponse> GetById(int boardId);
        Task<int> CreateBoard(CreateBoardRequest request, int userId);
        Task EditBoard(int boardId, EditBoardRequest request);
        Task DeleteBoard(int boardId);
        Task<IEnumerable<BoardResponse>> ListBoards();
    }
}