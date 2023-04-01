namespace FusionPlannerAPI.Boundary.Request
{
    public class MoveCardRequestObject
    {
        public int CardId { get; set; }
        public int SourceColumnId { get; set; }
        public int DestinationColumnId { get; set; }
        public int DestinationCardIndex { get; set; }
    }
}
