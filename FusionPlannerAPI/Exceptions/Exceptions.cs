namespace FusionPlannerAPI.Exceptions
{
    public class BoardNotFoundException : Exception
    {
        public readonly int BoardId;

        public BoardNotFoundException(int boardId)
        {
            BoardId = boardId;
        }
    }

    public class UserNotFoundException : Exception
    {
        public readonly int UserId;

        public UserNotFoundException(int userId)
        {
            UserId = userId;
        }
    }

    public class ColumnNotFoundException : Exception
    {
        public readonly int ColumnId;

        public ColumnNotFoundException(int columnId)
        {
            ColumnId = columnId;
        }
    }

    public class CardNotFoundException : Exception
    {
        public readonly int CardId;

        public CardNotFoundException(int cardId)
        {
            CardId = cardId;
        }
    }
}
