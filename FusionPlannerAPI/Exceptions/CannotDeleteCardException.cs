namespace FusionPlannerAPI.Exceptions
{
    public class CannotDeleteCardException : Exception
    {
        public CannotDeleteCardException()
            : base("Cards can only be deleted when archived.")
        {
        }
    }
}
