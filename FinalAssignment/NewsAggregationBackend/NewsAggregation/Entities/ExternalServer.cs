using NewsAggregation.Configurations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsAggregation.Entities
{
    public class ExternalServer : BaseAuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExternalServerId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ServerName { get; set; }

        [Required]
        [MaxLength(255)]
        public string BaseUrl { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        [MaxLength(255)]
        public string ApiKeyHash { get; set; }
    }
}
