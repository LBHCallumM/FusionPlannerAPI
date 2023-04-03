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

        public Task<int> CreateColumn(CreateColumnRequest request)
        {
            return _gateway.CreateColumn(request);
        }

        public Task ArchiveColumn(int columnId)
        {
            return _gateway.ArchiveColumn(columnId);
        }

        public Task RestoreColumn(int columnId)
        {
            return _gateway.RestoreColumn(columnId);
        }

        public Task EditColumn(int columnId, EditColumnRequest request)
        {
            return _gateway.EditColumn(columnId, request);
        }

        public Task<ColumnResponse> GetById(int columnId)
        {
            return _gateway.GetById(columnId);
        }

        public Task<IEnumerable<ColumnResponse>> ListColumns(int boardId)
        {
            return _gateway.ListColumns(boardId);
        }

        public Task MoveCard(MoveCardRequestObject request)
        {
            return _gateway.MoveCard(request);
        }
    }
}
