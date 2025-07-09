using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace NewsAggregation.Configurations.DatabaseConfigurations
{
    public class AuditDbBaseContext : DbContext
    {
        private readonly RequestContext _requestContext;
        public AuditDbBaseContext(DbContextOptions options, RequestContext requestContext) : base(options)
        {
            _requestContext = requestContext;
        }
        private void SetAuditProperties()
        {
            List<EntityEntry> updatedEntries = this.ChangeTracker.Entries()
                .Where(entry => entry.State == EntityState.Added || entry.State == EntityState.Modified)
                .Where(entry => entry.Entity is BaseAuditEntity).ToList();

            foreach (EntityEntry entry in updatedEntries)
            {
                var entity = entry.Entity as BaseAuditEntity;
                if (entity != null)
                {
                    if (entry.State == EntityState.Added)
                    {
                        entity.CreatedDateTime = DateTime.UtcNow;
                        entity.CreatedBy = _requestContext.Email;
                    }
                    entity.ModifiedDateTime = DateTime.UtcNow;
                    entity.ModifiedBy = _requestContext.Email;
                }
            }
        }

        public override int SaveChanges()
        {
            SetAuditProperties();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SetAuditProperties();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            SetAuditProperties();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
