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
        public DbSet<Keywords> Keywords { get; set; }
        public DbSet<UserKeyword> UserKeywords { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<UserArticleReaction> UserArticleReactions { get; set; }
        public DbSet<UserSavedArticle> UserSavedArticles { get; set; }
        public DbSet<UserNotificationConfiguration> UserNotificationConfigurations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserFluentConfiguration());
            modelBuilder.ApplyConfiguration(new ArticleFluentConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryFluentConfiguration());

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

            modelBuilder.Entity<UserNotification>()
                .HasOne(un => un.User)
                .WithMany(u => u.UserNotifications)
                .HasForeignKey(un => un.UserId);

            modelBuilder.Entity<UserNotification>()
                .HasOne(un => un.Article)
                .WithMany()
                .HasForeignKey(un => un.ArticleId);

            modelBuilder.Entity<UserNotificationConfiguration>()
                .HasOne(unc => unc.User)
                .WithMany(u => u.UserNotificationConfigurations)
                .HasForeignKey(unc => unc.UserId);

            modelBuilder.Entity<UserNotificationConfiguration>()
                .HasOne(unc => unc.Category)
                .WithMany()
                .HasForeignKey(unc => unc.CategoryId);

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


            base.OnModelCreating(modelBuilder);
        }
    }
}
