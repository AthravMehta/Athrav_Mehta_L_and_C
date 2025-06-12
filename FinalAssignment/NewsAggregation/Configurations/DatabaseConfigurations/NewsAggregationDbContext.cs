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
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ExternalServer> ExternalServers { get; set; }
        public DbSet<UserKeyword> UserKeywords { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<UserArticleAction> UserArticleActions { get; set; }
        public DbSet<UserNotificationConfiguration> UserNotificationConfigurations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserFluentConfiguration());
            modelBuilder.ApplyConfiguration(new ArticleFluentConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryFluentConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
