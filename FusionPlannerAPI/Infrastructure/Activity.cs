using System.ComponentModel.DataAnnotations;

namespace FusionPlannerAPI.Infrastructure
{
    public class Activity
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int AuthorId { get; set; }
        public User Author { get; set; }
        public int CardId { get; set; }
        public Card Card { get; set; }
        public int? CommentId { get; set; }
        public Comment? Comment { get; set; }
        public int? TransactionId { get; set; }
        public Transaction? Transaction { get; set; }
    }
}
