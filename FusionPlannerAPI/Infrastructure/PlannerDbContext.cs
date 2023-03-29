using Microsoft.EntityFrameworkCore;

namespace FusionPlannerAPI.Infrastructure
{
    public class PlannerDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<List> Lists { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
