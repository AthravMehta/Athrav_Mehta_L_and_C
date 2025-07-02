using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NewsAggregation.Entities;

namespace NewsAggregation.Configurations.FluentApiConfigurations
{
    public class ArticleFluentConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.ToTable("Articles");

            builder.HasOne<ExternalServer>()
               .WithMany()
               .HasForeignKey(a => a.ExternalServerId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Category>()
                .WithMany()
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.UserSavedArticles)
                .WithOne(ua => ua.Article)
                .HasForeignKey(ua => ua.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(a => a.UserArticleReactions)
                .WithOne(ua => ua.Article)
                .HasForeignKey(ua => ua.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(a => a.UserArticleReports)
                .WithOne(ua => ua.Article)
                .HasForeignKey(ua => ua.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
