namespace FusionPlannerAPI.Exceptions
{
    public class IndexOutOfRangeException : Exception
    {
        public IndexOutOfRangeException(int value)
            : base($"DestinationCardIndex with a value of '{value}' is out of range")
        {
        }
    }
}
