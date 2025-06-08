using Microsoft.EntityFrameworkCore;
using NewsAggregation.Configurations.FluentApiConfigurations;
using NewsAggregation.Entities;

namespace NewsAggregation.Configurations.DatabaseConfigurations
{
    public class NewsAggregationDbContext : AuditDbBaseContext
    {
        public NewsAggregationDbContext(DbContextOptions<NewsAggregationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserFluentConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
