using Microsoft.EntityFrameworkCore;
using ProductManagement.Constants;
using ProductManagement.DbContexts;
using ProductManagement.Entities;
using ProductManagement.Exceptions;
using ProductManagement.Enums;
using ProductManagement.Repository.Contracts;

namespace ProductManagement.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductContext _productContext;

        public ProductRepository(ProductContext productContext)
        {
            _productContext = productContext;
        }

        public async Task<Product> GetProduct(int id)
        {
            Product product = await _productContext.Products.Where(product => product.Id == id).FirstOrDefaultAsync();
            if (product != null)
            {
                return product;
            }
            throw new ApiException(ErrorCode.NotFound ,ErrorConstants.ProductNotFound);
        }

        public async Task<List<Product>> GetAllProducts()
        {
            List<Product> products = await _productContext.Products.ToListAsync();
            if (products.Any())
            {
                return products;
            }
            throw new ApiException(ErrorCode.NotFound, ErrorConstants.ProductsNotFound);
        }
        public async Task AddProduct(Product product)
        {
            if (product == null)
            {
                throw new ApiException(ErrorConstants.ProductIsNull);
            }
            await _productContext.Products.AddAsync(product);
            await _productContext.SaveChangesAsync();
        }
        public async Task DeleteProduct(Product product)
        {
            if (product == null)
            {
                throw new ApiException(ErrorConstants.ProductIsNull);
            }
            _productContext.Products.Remove(product);
            await _productContext.SaveChangesAsync();
        }

        public async Task UpdateProduct(Product updatedProduct)
        {
            if (updatedProduct == null)
            {
                throw new ApiException(ErrorConstants.UpdatedProductIsNull);
            }
            _productContext.Products.Update(updatedProduct);
            await _productContext.SaveChangesAsync();
        }

        public bool ProductExists(string name)
        {
            return _productContext.Products.Any(product => product.Name == name);
        }
    }
}
