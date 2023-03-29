using System.ComponentModel.DataAnnotations;

namespace FusionPlannerAPI.Infrastructure
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime LastEditedAt { get; set; }
        public bool IsArchived { get; set; }
    }
}
