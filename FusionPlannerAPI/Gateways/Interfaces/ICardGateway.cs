using FusionPlannerAPI.Boundary.Request;
using FusionPlannerAPI.Boundary.Response;
using FusionPlannerAPI.Infrastructure;

namespace FusionPlannerAPI.Gateways.Interfaces
{
    public interface ICardGateway
    {
        Task<CardResponse> GetById(int cardId);
        Task<int> CreateCard(CreateCardRequest request, int createdById);
        Task EditCard(int cardId, EditCardRequest request);
        Task DeleteCard(int cardId);
        Task ArchiveCard(int cardId);
        Task RestoreCard(int cardId);
    }
}