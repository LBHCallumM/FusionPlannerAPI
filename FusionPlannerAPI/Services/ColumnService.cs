using FusionPlannerAPI.Boundary.Request;
using FusionPlannerAPI.Boundary.Response;
using FusionPlannerAPI.Gateways.Interfaces;
using FusionPlannerAPI.Services.Interfaces;

namespace FusionPlannerAPI.Services
{
    public class ColumnService : IColumnService
    {
        private readonly IColumnGateway _gateway;

        public ColumnService(IColumnGateway gateway)
        {
            _gateway = gateway;
        }

        public async Task<int> CreateColumn(CreateColumnRequest request)
        {
            return await _gateway.CreateColumn(request);
        }

        public async Task DeleteColumn(int columnId)
        {
            await _gateway.DeleteColumn(columnId);
        }

        public async Task EditColumn(int columnId, EditColumnRequest request)
        {
            await _gateway.EditColumn(columnId, request);
        }

        public async Task<ColumnResponse> GetById(int columnId)
        {
            return await _gateway.GetById(columnId);
        }

        public async Task<IEnumerable<ColumnResponse>> ListColumns(int boardId)
        {
            return await _gateway.ListColumns(boardId);
        }

        public async Task MoveCard(MoveCardRequestObject request)
        {
            await _gateway.MoveCard(request);
        }
    }
}
