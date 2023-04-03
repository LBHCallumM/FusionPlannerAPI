namespace FusionPlannerAPI.Exceptions
{
    public class CardAlreadyInPositionException : Exception
    {
        public CardAlreadyInPositionException()
            : base("Card already in specified position")
        {
        }
    }
}
