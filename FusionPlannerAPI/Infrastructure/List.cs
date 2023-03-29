using System.ComponentModel.DataAnnotations;

namespace FusionPlannerAPI.Infrastructure
{
    public class List
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int BoardId { get; set; }
        public Board Board { get; set; }
        public List<int> CardOrder { get; set; }
        public int CardOrderVersion { get; set; }
        public ICollection<Card> Cards { get; set; }
    }
}
