using System.ComponentModel.DataAnnotations;

namespace FusionPlannerAPI.Infrastructure
{
    public class Card
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ColumnId { get; set; }
        public virtual Column Column { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastEditedAt { get; set; }
        public int CreatedById { get; set; }
        public virtual User CreatedBy { get; set; }
        public bool IsArchived { get; set; }
        public int DisplayOrder { get; set; }
    }
}
