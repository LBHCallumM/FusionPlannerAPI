namespace FusionPlannerAPI.Exceptions
{
    public class CannotEditArchivedCardException : Exception
    {
        public CannotEditArchivedCardException()
            : base($"Cannot edit a card when archived")
        {
        }
    }
}
