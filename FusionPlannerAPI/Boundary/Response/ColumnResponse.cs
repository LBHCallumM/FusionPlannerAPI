using FusionPlannerAPI.Infrastructure;

namespace FusionPlannerAPI.Boundary.Response
{
    public class ColumnResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<CardResponse> Cards { get; set; }

        public ColumnResponse()
        {
            Cards = new List<CardResponse>();
        }
    }
}
