using System.ComponentModel.DataAnnotations;

namespace FusionPlannerAPI.Infrastructure
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public ICollection<Board> Boards { get; set; }
    }
}
