using FusionPlannerAPI.Boundary.Request;
using FusionPlannerAPI.Boundary.Response;
using FusionPlannerAPI.Exceptions;
using FusionPlannerAPI.Factories;
using FusionPlannerAPI.Gateways.Interfaces;
using FusionPlannerAPI.Infrastructure;
using FusionPlannerAPI.Services;

namespace FusionPlannerAPI.Gateways
{
    public class CardGateway : ICardGateway
    {
        private readonly PlannerDbContext _dbContext;

        public CardGateway(PlannerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreateCard(CreateCardRequest request, int createdById)
        {
            var column = await _dbContext.Columns.FindAsync(request.ColumnId);
            if (column == null) throw new ColumnNotFoundException(request.ColumnId);

            var user = await _dbContext.Users.FindAsync(createdById);
            if (user == null) throw new UserNotFoundException(createdById);

            var card = request.ToDatabase(createdById);

            _dbContext.Cards.Add(card);
            await _dbContext.SaveChangesAsync();

            return card.Id;
        }

        public async Task DeleteCard(int cardId)
        {
            var card = await _dbContext.Cards.FindAsync(cardId);
            if (card == null) throw new CardNotFoundException(cardId);

            _dbContext.Cards.Remove(card);
            await _dbContext.SaveChangesAsync();
        }

        public async Task EditCard(int cardId, EditCardRequest request)
        {
            var card = await _dbContext.Cards.FindAsync(cardId);
            if (card == null) throw new CardNotFoundException(cardId);

            card.Name = request.Name;
            card.Description = request.Description;
            card.LastEditedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<CardResponse> GetById(int cardId)
        {
            var card = await _dbContext.Cards.FindAsync(cardId);
            if (card == null) return null;

            return card.ToResponse();
        }
    }
}
