using Microsoft.EntityFrameworkCore;
using ProductManagement.Constants;
using ProductManagement.Entities;
using ProductManagement.Repository.Contracts;
using ProductManagement.Services.Contracts;
using System.Net;

namespace ProductManagement.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product> GetProduct(int id)
        {
            if (id == 0)
            {
                throw new Exception(ErrorConstants.IdCannotBeZero);
            }

            Product product = await _productRepository.GetProduct(id);
            if (product == null)
            {
                throw new Exception($"{ErrorConstants.ProductNotFound}");
            }
            return product;
        }
        public async Task<List<Product>> GetAllProducts()
        {
            List<Product> products = await _productRepository.GetAllProducts();
            if (!products.Any())
            {
                throw new Exception($"{ErrorConstants.ProductsNotFound}");
            }
            return products;
        }

        public async Task AddProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            await _productRepository.AddProduct(product);
        }

        public async Task UpdateProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            Product oldProduct = await GetProduct(product.Id);
            oldProduct.Name = product.Name;
            oldProduct.Price = product.Price;
            oldProduct.Quantity = product.Quantity;
            oldProduct.ProductType = product.ProductType;
            await _productRepository.UpdateProduct(oldProduct);
        }

        public async Task DeleteProduct(int id)
        {
            if (id == 0)
            {
                throw new Exception(ErrorConstants.IdCannotBeZero);
            }
            Product productToDelete = await GetProduct(id);
            await _productRepository.DeleteProduct(productToDelete);
        }

        public bool ProductExists(string name)
        {
            return _productRepository.ProductExists(name);
        }
    }
}