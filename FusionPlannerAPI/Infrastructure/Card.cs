using System.ComponentModel.DataAnnotations;

namespace FusionPlannerAPI.Infrastructure
{
    public class Card
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ListId { get; set; }
        public List List { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastEditedAt { get; set; }
        public int CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public int AssignedToId { get; set; }
        public User AssignedTo { get; set; }
        public bool IsArchived { get; set; }
        public ICollection<Activity> Activities { get; set; }
        public ICollection<CardLabel> CardLabels { get; set; }
    }
}
