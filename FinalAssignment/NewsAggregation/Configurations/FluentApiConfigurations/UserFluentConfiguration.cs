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
        }
    }
}
