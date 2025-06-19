using NewsAggregation.Configurations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewsAggregation.Entities
{
    public class Category : BaseAuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public ICollection<Article> Articles { get; set; }
        public ICollection<Keywords> Keywords { get; set; }
        public ICollection<UserKeyword> UserKeywords { get; set; }
    }
}
