using System.ComponentModel.DataAnnotations;

namespace NewsAggregation.Configurations
{
    public abstract class BaseAuditEntity
    {
        public DateTime CreatedDateTime { get; set; }

        public DateTime ModifiedDateTime { get; set; }

        [MaxLength(1024)]
        public string? CreatedBy { get; set; }

        [MaxLength(1024)]
        public string? ModifiedBy { get; set; }

        public BaseAuditEntity()
        {
            CreatedDateTime = DateTime.UtcNow;
            ModifiedDateTime = DateTime.UtcNow;
        }
    }
}
