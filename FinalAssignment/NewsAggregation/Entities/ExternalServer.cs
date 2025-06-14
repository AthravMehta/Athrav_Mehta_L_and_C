using NewsAggregation.Configurations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsAggregation.Entities
{
    public class ExternalServer : BaseAuditEntity, BaseKeyEntity<Guid>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string ServerName { get; set; }

        [Required]
        [MaxLength(255)]
        public string BaseUrl { get; set; }

        // TODO: Make casing correct in next migration
        [Required]
        public bool isActive { get; set; }

        [Required]
        [MaxLength(255)]
        public string ApiKeyHash { get; set; }
    }
}
