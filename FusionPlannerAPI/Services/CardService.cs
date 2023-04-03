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

        public async Task ArchiveCard(int cardId)
        {
            await _gateway.ArchiveCard(cardId);
        }

        public async Task<int> CreateCard(CreateCardRequest request, int createdById)
        {
            return await _gateway.CreateCard(request, createdById);
        }

        public async Task DeleteCard(int cardId)
        {
            await _gateway.DeleteCard(cardId);
        }

        public async Task EditCard(int cardId, EditCardRequest request)
        {
            await _gateway.EditCard(cardId, request);
        }

        public async Task<CardResponse> GetById(int cardId)
        {
            return await _gateway.GetById(cardId);
        }
    }
}
