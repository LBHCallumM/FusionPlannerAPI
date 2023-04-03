using FusionPlannerAPI.Boundary.Request;
using FusionPlannerAPI.Boundary.Response;

namespace FusionPlannerAPI.Services.Interfaces
{
    public interface ICardService
    {
        Task<CardResponse> GetById(int cardId);
        Task<int> CreateCard(CreateCardRequest request, int createdById);
        Task EditCard(int cardId, EditCardRequest request);
        Task DeleteCard(int cardId);
        Task ArchiveCard(int cardId);
    }
}