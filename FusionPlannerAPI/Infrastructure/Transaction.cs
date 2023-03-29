using System.ComponentModel.DataAnnotations;

namespace FusionPlannerAPI.Infrastructure
{

    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
