namespace FusionPlannerAPI.Exceptions
{
    public class CardNotFoundException : Exception
    {
        public CardNotFoundException(int cardId)
            : base($"Card not found with id {cardId}")
        {
        }
    }
}
