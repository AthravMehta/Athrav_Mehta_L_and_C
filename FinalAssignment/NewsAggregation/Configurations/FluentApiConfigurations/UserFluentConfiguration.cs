using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsAggregation.Entities;

namespace NewsAggregation.Configurations.FluentApiConfigurations
{
    public class UserFluentConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(user => user.Username).IsUnique();
            builder.HasIndex(user => user.Email).IsUnique();

            builder.HasMany<UserSavedArticle>()
                .WithOne(ua => ua.User)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany<UserArticleReaction>()
                .WithOne(ua => ua.User)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany<UserNotification>()
                .WithOne()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany<UserNotificationConfiguration>()
                .WithOne()
                .HasForeignKey(nc => nc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany<UserKeyword>()
                .WithOne()
                .HasForeignKey(uk => uk.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
