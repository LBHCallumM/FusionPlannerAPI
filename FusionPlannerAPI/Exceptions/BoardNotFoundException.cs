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
}
