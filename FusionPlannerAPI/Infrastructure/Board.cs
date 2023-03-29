using System.ComponentModel.DataAnnotations;

namespace FusionPlannerAPI.Infrastructure
{
    public class Board
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int OwnerId { get; set; }
        public User Owner { get; set; }
        public List<int> ListOrder { get; set; }
        public int ListOrderVersion { get; set; }
        public ICollection<List> Lists { get; set; }
    }
}
