using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace FusionPlannerAPI.Infrastructure
{
    public class PlannerDbContext: DbContext
    {
        public PlannerDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Column> Columns { get; set; }
        public DbSet<Card> Cards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Board>()
                .HasMany(b => b.Columns)
                .WithOne(c => c.Board)
                .HasForeignKey(c => c.BoardId);

            modelBuilder.Entity<Column>()
                .HasMany(c => c.Cards)
                .WithOne(card => card.Column)
                .HasForeignKey(card => card.ColumnId);

            modelBuilder.Entity<Card>()
                .HasIndex(card => new { card.ColumnId, card.DisplayOrder })
                .IsUnique();

            modelBuilder.Entity<Column>()
                .HasOne(c => c.Board)
                .WithMany(b => b.Columns)
                .HasForeignKey(c => c.BoardId)
                .IsRequired();
        }

    }
}
