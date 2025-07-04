using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NewsAggregation.Entities;
using NewsAggregation.NewsAggregation.Entities;

namespace NewsAggregation.NewsAggregation.Configurations.FluentApiConfigurations
{
    public class UserArticleReadTrackingConfiguration : IEntityTypeConfiguration<UserArticleReadTracking>
    {
        public void Configure(EntityTypeBuilder<UserArticleReadTracking> builder)
        {
            builder.ToTable("UserArticleReadTrackings");
            builder.HasKey(e => e.UserArticleReadTrackingId);

            builder.Property(e => e.UserId)
                .IsRequired();

            builder.Property(e => e.ArticleId)
                .IsRequired();

            builder.Property(e => e.CategoryId)
                .IsRequired();

            builder.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);

            builder.HasOne(e => e.Article)
                .WithMany()
                .HasForeignKey(e => e.ArticleId);

            builder.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId);

            builder.HasIndex(e => e.UserId);
            builder.HasIndex(e => e.ArticleId);
            builder.HasIndex(e => e.CategoryId);


        }
    }
}
