using FusionPlannerAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FusionPlannerAPI.Tests
{
    public static class InMemoryDb
    {
        private static PlannerDbContext _context;

        public static PlannerDbContext Instance
        {
            get
            {
                if (_context == null)
                {
                    DbContextOptionsBuilder<PlannerDbContext> builder = new DbContextOptionsBuilder<PlannerDbContext>();
                    builder.EnableSensitiveDataLogging();
                    builder.UseLazyLoadingProxies();
                    builder.ConfigureWarnings(options =>
                    {
                        options.Ignore(InMemoryEventId.TransactionIgnoredWarning);
                    });

                    //Mimic behaviour in .NET Core 3.1 (don't check null values)
                    builder.UseInMemoryDatabase(Guid.NewGuid().ToString(), db => db.EnableNullChecks(false));

                    _context = new PlannerDbContext(builder.Options);
                    _context.Database.EnsureCreated();
                }

                return _context;
            }
        }

        //public static ITransactionManager TransactionManager => new TransactionManager(Instance);

        public static void Teardown()
        {
            _context = null;
        }
    }
}
