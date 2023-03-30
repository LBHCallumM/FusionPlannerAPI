namespace FusionPlannerAPI.Exceptions
{
    public class BoardNotFoundException : Exception
    {
        private int boardId;

        public BoardNotFoundException(int boardId)
        {
            this.boardId = boardId;
        }
    }

    public class UserNotFoundException : Exception
    {
        private int userId;

        public UserNotFoundException(int userId)
        {
            this.userId = userId;
        }
    }

    public class ColumnNotFoundException : Exception
    {
        private int columnId;

        public ColumnNotFoundException(int columnId)
        {
            this.columnId = columnId;
        }
    }

    public class CardNotFoundException : Exception
    {
        private int cardId;

        public CardNotFoundException(int cardId)
        {
            this.cardId = cardId;
        }
    }
}
