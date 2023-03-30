using FusionPlannerAPI.Boundary.Request;
using FusionPlannerAPI.Boundary.Response;
using FusionPlannerAPI.Infrastructure;

namespace FusionPlannerAPI.Gateways.Interfaces
{
    public interface IColumnGateway
    {
        Task<ColumnResponse> GetById(int columnId);
        Task<IEnumerable<ColumnResponse>> ListColumns(int boardId);
        Task<int> CreateColumn(CreateColumnRequest request);
        Task EditColumn(int columnId, EditColumnRequest request);
        Task DeleteColumn(int columnId);
    }
}