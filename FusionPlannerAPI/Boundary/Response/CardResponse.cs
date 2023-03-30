namespace FusionPlannerAPI.Boundary.Response
{
    public class CardResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ColumnId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastEditedAt { get; set; }
        public int CreatedById { get; set; }
        public int DisplayOrder { get; set; }
    }
}
