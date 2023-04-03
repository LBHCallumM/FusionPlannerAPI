namespace FusionPlannerAPI.Exceptions
{
    public class ColumnNotFoundException : Exception
    {
        public ColumnNotFoundException(int columnId)
            : base($"Column not found with id {columnId}")
        {
        }
    }
}
