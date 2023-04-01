using FusionPlannerAPI.Boundary.Request;
using FusionPlannerAPI.Boundary.Response;

namespace FusionPlannerAPI.Services.Interfaces
{
    public interface IColumnService
    {
        Task<ColumnResponse> GetById(int columnId);
        Task<IEnumerable<ColumnResponse>> ListColumns(int boardId);
        Task<int> CreateColumn(CreateColumnRequest request);
        Task EditColumn(int columnId, EditColumnRequest request);
        Task DeleteColumn(int columnId);
        Task MoveCard(MoveCardRequestObject request);
    }
}