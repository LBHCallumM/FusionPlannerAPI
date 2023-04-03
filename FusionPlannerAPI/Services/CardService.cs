using FusionPlannerAPI.Boundary.Request;
using FusionPlannerAPI.Boundary.Response;
using FusionPlannerAPI.Gateways.Interfaces;
using FusionPlannerAPI.Infrastructure;
using FusionPlannerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace FusionPlannerAPI.Services
{
    public class CardService : ICardService
    {
        private readonly ICardGateway _gateway;

        public CardService(ICardGateway cardGateway)
        {
            _gateway = cardGateway;
        }

        public Task ArchiveCard(int cardId)
        {
            return _gateway.ArchiveCard(cardId);
        }

        public Task<int> CreateCard(CreateCardRequest request, int createdById)
        {
            return _gateway.CreateCard(request, createdById);
        }

        public Task DeleteCard(int cardId)
        {
            return _gateway.DeleteCard(cardId);
        }

        public Task EditCard(int cardId, EditCardRequest request)
        {
            return _gateway.EditCard(cardId, request);
        }

        public Task<CardResponse> GetById(int cardId)
        {
            return _gateway.GetById(cardId);
        }

        public Task RestoreCard(int cardId)
        {
            return _gateway.RestoreCard(cardId);
        }
    }
}
