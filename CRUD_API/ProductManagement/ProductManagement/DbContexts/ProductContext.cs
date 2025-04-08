using Microsoft.EntityFrameworkCore;
using ProductManagement.Entities;

namespace ProductManagement.DbContexts
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
