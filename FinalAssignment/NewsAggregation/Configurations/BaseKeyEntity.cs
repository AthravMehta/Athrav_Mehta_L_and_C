using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsAggregation.Configurations
{
    public interface BaseKeyEntity<TKey>
    {
        public TKey Id { get; set; }
    }
}
