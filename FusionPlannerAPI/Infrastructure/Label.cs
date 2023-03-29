using System.ComponentModel.DataAnnotations;

namespace FusionPlannerAPI.Infrastructure
{
    public class Label
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<CardLabel> CardLabels { get; set; }
    }
}
