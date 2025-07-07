using Microsoft.EntityFrameworkCore;
using NewsAggregation.Configurations.FluentApiConfigurations;
using NewsAggregation.Entities;
using NewsAggregation.NewsAggregation.Configurations.FluentApiConfigurations;

namespace NewsAggregation.Configurations.DatabaseConfigurations
{
    public class NewsAggregationDbContext : AuditDbBaseContext
    {
        public NewsAggregationDbContext(DbContextOptions<NewsAggregationDbContext> options, RequestContext requestContext) : base(options, requestContext)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ExternalServer> ExternalServers { get; set; }
        public DbSet<Keywords> Keywords { get; set; }
        public DbSet<UserKeyword> UserKeywords { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<UserArticleReaction> UserArticleReactions { get; set; }
        public DbSet<UserSavedArticle> UserSavedArticles { get; set; }
        public DbSet<UserArticleReport> UserArticleReports { get; set; }
        public DbSet<UserNotificationConfiguration> UserNotificationConfigurations { get; set; }
        public DbSet<UserArticleReadTracking> UserArticleReadTrackings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserFluentConfiguration());
            modelBuilder.ApplyConfiguration(new ArticleFluentConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryFluentConfiguration());
            modelBuilder.ApplyConfiguration(new UserArticleReadTrackingConfiguration());

            modelBuilder.Entity<Keywords>()
                .HasOne(k => k.Category)
                .WithMany(k => k.Keywords)
                .HasForeignKey(k => k.CategoryId);

            modelBuilder.Entity<UserKeyword>()
                .HasOne(uk => uk.User)
                .WithMany(u => u.UserKeywords)
                .HasForeignKey(uk => uk.UserId);

            modelBuilder.Entity<UserKeyword>()
                .HasOne(uk => uk.Category)
                .WithMany(c => c.UserKeywords)
                .HasForeignKey(uk => uk.CategoryId);

            modelBuilder.Entity<UserNotification>(entity =>
            {
                entity.HasKey(e => e.UserNotificationId);

                modelBuilder.Entity<UserNotification>()
                    .HasOne(un => un.User)
                    .WithMany(u => u.UserNotifications)
                    .HasForeignKey(un => un.UserId);

                modelBuilder.Entity<UserNotification>()
                    .HasOne(un => un.Article)
                    .WithMany()
                    .HasForeignKey(un => un.ArticleId);
            });

            modelBuilder.Entity<UserNotificationConfiguration>(entity =>
            {
                entity.HasKey(e => e.UserNotificationConfigurationId);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.UserNotificationConfigurations)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Category)
                    .WithMany()
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserSavedArticle>()
                .HasOne(usa => usa.User)
                .WithMany(u => u.UserSavedArticles)
                .HasForeignKey(usa => usa.UserId);

            modelBuilder.Entity<UserSavedArticle>()
                .HasOne(usa => usa.Article)
                .WithMany(a => a.UserSavedArticles)
                .HasForeignKey(usa => usa.ArticleId);

            modelBuilder.Entity<UserArticleReaction>()
                .HasOne(uar => uar.User)
                .WithMany()
                .HasForeignKey(uar => uar.UserId);

            modelBuilder.Entity<UserArticleReaction>()
                .HasOne(uar => uar.Article)
                .WithMany(a => a.UserArticleReactions)
                .HasForeignKey(uar => uar.ArticleId);

            modelBuilder.Entity<UserArticleReport>()
                .HasOne(uar => uar.User)
                .WithMany(uar => uar.UserArticleReports)
                .HasForeignKey(uar => uar.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserArticleReport>()
                .HasOne(uar => uar.Article)
                .WithMany(uar => uar.UserArticleReports)
                .HasForeignKey(uar => uar.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>().HasData(SeedData.GetCategories());
            modelBuilder.Entity<Keywords>().HasData(SeedData.GetKeywords());

            base.OnModelCreating(modelBuilder);
        }
    }
}
