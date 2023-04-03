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
using IndexOutOfRangeException = FusionPlannerAPI.Exceptions.IndexOutOfRangeException;

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

            foreach (var column in columns)
            {
                column.Cards = column.Cards.Where(x => x.IsArchived == false).ToList();
            }

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
            var card = await _dbContext.Cards.FindAsync(request.CardId);
            if (card == null) throw new CardNotFoundException(request.CardId);

            if (CardAlreadyInPosition(request, card))
            {
                throw new CardAlreadyInPositionException();
            }

            var sourceList = await _dbContext.Columns.FindAsync(request.SourceColumnId);
            if (sourceList == null) throw new ColumnNotFoundException(request.SourceColumnId);


            // Check if the card is being moved to a different list
            if (request.SourceColumnId != request.DestinationColumnId)
            {
                await MoveCardToDifferentColumn(request, card, sourceList);
            }
            else
            {
                MoveCardWithinSameColumn(request, card, sourceList);
            }

            await _dbContext.SaveChangesAsync();
        }

        private static void CheckDestinationIndexInRange(MoveCardRequestObject request, Column destinationColumn)
        {
            if (request.DestinationCardIndex < 0 || request.DestinationCardIndex > destinationColumn.Cards.Count)
            {
                throw new IndexOutOfRangeException(request.DestinationCardIndex);
            }
        }


        private async Task MoveCardToDifferentColumn(MoveCardRequestObject request, Card card, Column sourceList)
        {
            var destinationList = await _dbContext.Columns.FindAsync(request.DestinationColumnId);
            if (destinationList == null) throw new ColumnNotFoundException(request.DestinationColumnId);

            CheckDestinationIndexInRange(request, destinationList);

            sourceList.Cards.Remove(card);
            destinationList.Cards.Insert(request.DestinationCardIndex, card);
            card.ColumnId = destinationList.Id;


            UpdateDisplayOrders(sourceList);
            UpdateDisplayOrders(destinationList);

            CheckForDuplicateDisplayOrderValues(sourceList);
            CheckForDuplicateDisplayOrderValues(destinationList);
        }

        private static void UpdateDisplayOrders(Column column)
        {
            for (int i = 0; i < column.Cards.Count; i++)
            {
                column.Cards.ElementAt(i).DisplayOrder = i + 1;
            }
        }

        private static void MoveCardWithinSameColumn(MoveCardRequestObject request, Card card, Column sourceColumn)
        {
            CheckDestinationIndexInRange(request, sourceColumn);

            // Get a list of all available indexes in the column, except for the destination index
            var availableIndexes = Enumerable.Range(0, sourceColumn.Cards.Count)
                .Where(i => i != request.DestinationCardIndex)
                .ToList();

            var indexQueue = new Queue<int>(availableIndexes);

            card.DisplayOrder = request.DestinationCardIndex + 1;

            foreach (var x in sourceColumn.Cards.OrderBy(c => c.DisplayOrder))
            {
                if (x.Id != request.CardId)
                {
                    // Update the display order of the other cards using the next available index
                    x.DisplayOrder = indexQueue.Dequeue() + 1;
                }
            }

            CheckForDuplicateDisplayOrderValues(sourceColumn);
        }


        private static void CheckForDuplicateDisplayOrderValues(Column column)
        {
            var displayOrderValues = column.Cards.Select(x => x.DisplayOrder).ToList();

            if (displayOrderValues.Count != displayOrderValues.Distinct().Count())
            {
                throw new DuplicateDisplayOrderValuesException(displayOrderValues);
            }
        }

        private static bool CardAlreadyInPosition(MoveCardRequestObject request, Card card)
        {
            return card.DisplayOrder == request.DestinationCardIndex +1
                && request.SourceColumnId == request.DestinationColumnId;
        }
    }
}
