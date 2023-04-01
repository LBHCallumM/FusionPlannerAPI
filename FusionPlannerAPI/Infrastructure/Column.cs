using System.ComponentModel.DataAnnotations;

namespace FusionPlannerAPI.Infrastructure
{
    public class Column
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int BoardId { get; set; }
        public virtual Board Board { get; set; }
        public virtual List<Card> Cards { get; set; }
    }
}
