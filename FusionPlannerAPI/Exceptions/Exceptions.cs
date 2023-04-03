using FusionPlannerAPI.Infrastructure;

namespace FusionPlannerAPI.Exceptions
{
    public class BoardNotFoundException : Exception
    {
        public BoardNotFoundException(int boardId)
            : base($"Board not found with id {boardId}")
        {
        }
    }

    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(int userId)
            : base($"User not found with id {userId}")
        {
        }
    }

    public class ColumnNotFoundException : Exception
    {
        public ColumnNotFoundException(int columnId)
            : base($"Column not found with id {columnId}")
        {
        }
    }

    public class CardNotFoundException : Exception
    {
        public CardNotFoundException(int cardId)
            : base($"Card not found with id {cardId}")
        {
        }
    }

    public class IndexOutOfRangeException : Exception
    {
        public IndexOutOfRangeException(int value)
            : base($"DestinationCardIndex with a value of '{value}' is out of range")
        {
        }
    }

    public class CardAlreadyInPositionException : Exception
    {
        public CardAlreadyInPositionException()
            : base("Card already in specified position")
        {
        }
    }



    public class DuplicateDisplayOrderValuesException: Exception
    {
        public readonly List<int> DisplayOrderValues;

        public DuplicateDisplayOrderValuesException(List<int> displayOrderValues)
        {
            DisplayOrderValues = displayOrderValues;
        }
    }
}
