namespace FusionPlannerAPI.Exceptions
{
    public class CannotEditArchivedColumnException : Exception
    {
        public CannotEditArchivedColumnException()
            : base($"Cannot edit a column when archived")
        {
        }
    }
}
