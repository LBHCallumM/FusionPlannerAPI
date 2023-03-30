using FusionPlannerAPI.Boundary.Request;
using FusionPlannerAPI.Infrastructure;
using System.Net.NetworkInformation;

namespace FusionPlannerAPI.Factories
{
    public static class DatabaseFactory
    {
        public static Board ToDatabase(this CreateBoardRequest request, int ownerId)
        {
            return new Board
            {
                Name = request.Name,
                Description = request.Description,
                OwnerId = ownerId
            };
        }

        public static Column ToDatabase(this CreateColumnRequest request)
        {
            return new Column
            {
                Name = request.Name,
                BoardId = request.BoardId
            };
        }

        public static Card ToDatabase(this CreateCardRequest request, int createdById)
        {
            return new Card
            {
                Name = request.Name,
                Description = request.Description,
                ColumnId = request.ColumnId,
                CreatedAt = DateTime.UtcNow,
                LastEditedAt = DateTime.UtcNow,
                CreatedById = createdById,
                IsArchived = false,
                DisplayOrder = request.DisplayOrder
            };
        }
    }
}
