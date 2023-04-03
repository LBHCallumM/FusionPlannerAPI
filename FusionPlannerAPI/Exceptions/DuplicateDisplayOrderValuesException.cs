namespace FusionPlannerAPI.Exceptions
{
    public class DuplicateDisplayOrderValuesException: Exception
    {
        public readonly List<int> DisplayOrderValues;

        public DuplicateDisplayOrderValuesException(List<int> displayOrderValues)
        {
            DisplayOrderValues = displayOrderValues;
        }
    }
}
