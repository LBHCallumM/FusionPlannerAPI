using FusionPlannerAPI.Boundary.Request;
using FusionPlannerAPI.Boundary.Response;
using FusionPlannerAPI.Exceptions;
using FusionPlannerAPI.Factories;
using FusionPlannerAPI.Gateways.Interfaces;
using FusionPlannerAPI.Infrastructure;
using FusionPlannerAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Linq;
using Column = FusionPlannerAPI.Infrastructure.Column;

namespace FusionPlannerAPI.Gateways
{
    public class ColumnGateway : IColumnGateway
    {
        private readonly PlannerDbContext _dbContext;

        public ColumnGateway(PlannerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ColumnResponse> GetById(int columnId)
        {
            var column = await _dbContext.Columns.FindAsync(columnId);
            if (column == null) return null;

            return column.ToResponse();
        }

        public async Task<IEnumerable<ColumnResponse>> ListColumns(int boardId)
        {
            var board = await _dbContext.Boards.FindAsync(boardId);
            if (board == null) throw new BoardNotFoundException(boardId);

            var columns = await _dbContext.Columns
                .Where(x => x.BoardId == boardId)
                .Include(x => x.Cards)
                .ToListAsync();

            return columns.ToResponse();
        }

        public async Task<int> CreateColumn(CreateColumnRequest request)
        {
            var board = await _dbContext.Boards.FindAsync(request.BoardId);
            if (board == null) throw new BoardNotFoundException(request.BoardId);

            var column = request.ToDatabase();

            _dbContext.Columns.Add(column);
            await _dbContext.SaveChangesAsync();

            return column.Id;
        }

        public async Task EditColumn(int columnId, EditColumnRequest request)
        {
            var column = await _dbContext.Columns.FindAsync(columnId);
            if (column == null) throw new ColumnNotFoundException(columnId);

            column.Name = request.Name;

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteColumn(int columnId)
        {
            var column = await _dbContext.Columns.FindAsync(columnId);
            if (column == null) throw new ColumnNotFoundException(columnId);

            // ToDo - Check for cards

            _dbContext.Columns.Remove(column);
            await _dbContext.SaveChangesAsync();
        }

        public async Task MoveCard(MoveCardRequestObject request)
        {

            // Add checks on destination index (destionation list has number of elemetns)
            // or greater than zero

            var card = await _dbContext.Cards.FindAsync(request.CardId);
            if (card == null) throw new CardNotFoundException(request.CardId);

            if (CardAlreadyInPosition(request, card))
            {
                return;
            }

            var sourceList = await _dbContext.Columns.FindAsync(request.SourceColumnId);
            if (sourceList == null) throw new ColumnNotFoundException(request.SourceColumnId);

            if (request.SourceColumnId == request.DestinationColumnId)
            {
                if (request.DestinationCardIndex < 0 || request.DestinationCardIndex >= sourceList.Cards.Count)
                {
                    throw new ArgumentException($"{nameof(request.DestinationCardIndex)} is out of range");
                }
            }

            sourceList.Cards.Remove(card);

            Column destinationList = null;

            // Check if the card is being moved to a different list
            if (request.SourceColumnId != request.DestinationColumnId)
            {
                destinationList = await _dbContext.Columns.FindAsync(request.DestinationColumnId);
                if (destinationList == null) throw new ColumnNotFoundException(request.DestinationColumnId);

                if (request.DestinationCardIndex < 0 || request.DestinationCardIndex >= destinationList.Cards.Count + 1)
                {
                    throw new ArgumentException($"{nameof(request.DestinationCardIndex)} is out of range");
                }

                destinationList.Cards.Insert(request.DestinationCardIndex, card);
                card.ColumnId = destinationList.Id;
            }
            else
            {
                // Move the card within the same list
                sourceList.Cards.Insert(request.DestinationCardIndex, card);
            }

            for (int i = 0; i < sourceList.Cards.Count; i++)
            {
                sourceList.Cards.ElementAt(i).DisplayOrder = i + 1;
            }

            if (destinationList != null)
            {
                for (int i = 0; i < destinationList.Cards.Count; i++)
                {
                    destinationList.Cards.ElementAt(i).DisplayOrder = i + 1;
                }

            }

            await _dbContext.SaveChangesAsync();
        }

        private static bool CardAlreadyInPosition(MoveCardRequestObject request, Card card)
        {
            return card.DisplayOrder == request.DestinationCardIndex && request.SourceColumnId == request.DestinationColumnId;
        }
    }
}
