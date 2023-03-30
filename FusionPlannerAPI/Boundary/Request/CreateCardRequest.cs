using FusionPlannerAPI.Infrastructure;

namespace FusionPlannerAPI.Boundary.Request
{
    public class CreateCardRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int ColumnId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
