namespace FusionPlannerAPI.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(int userId)
            : base($"User not found with id {userId}")
        {
        }
    }
}
