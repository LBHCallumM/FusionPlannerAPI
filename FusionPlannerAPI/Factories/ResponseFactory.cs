using FusionPlannerAPI.Boundary.Response;
using FusionPlannerAPI.Infrastructure;


namespace FusionPlannerAPI.Factories
{
    public static class ResponseFactory
    {
        public static BoardResponse ToResponse(this Board database)
        {
            return new BoardResponse
            {
                Id = database.Id,
                Name = database.Name,
                Description = database.Description,
                OwnerId = database.OwnerId
            };
        }

        public static IEnumerable<BoardResponse> ToResponse(this IEnumerable<Board> database)
        {
            return database.Select(x => x.ToResponse());
        }

        public static ColumnResponse ToResponse(this Column database)
        {
            return new ColumnResponse
            {
                Id = database.Id,
                Name = database.Name,
                Cards = database.Cards?.ToResponse()
            };
        }

        public static IEnumerable<ColumnResponse> ToResponse(this IEnumerable<Column> database)
        {
            return database.Select(x => x.ToResponse());
        }

        public static CardResponse ToResponse(this Card database)
        {
            return new CardResponse
            {
                Id = database.Id,
                Name = database.Name,
                Description = database.Description,
                ColumnId = database.ColumnId,
                CreatedAt = database.CreatedAt,
                LastEditedAt = database.LastEditedAt,
                CreatedById = database.CreatedById,
                DisplayOrder = database.DisplayOrder,
                IsArchived = database.IsArchived
            };
        }

        public static IEnumerable<CardResponse> ToResponse(this IEnumerable<Card> database)
        {
            return database.Select(x => x.ToResponse());
        }
    }
}
