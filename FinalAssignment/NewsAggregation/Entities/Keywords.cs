using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using NewsAggregation.Configurations;

namespace NewsAggregation.Entities
{
    public class Keywords : BaseAuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int KeywordId { get; set; }
        public string Keyword { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }
}
