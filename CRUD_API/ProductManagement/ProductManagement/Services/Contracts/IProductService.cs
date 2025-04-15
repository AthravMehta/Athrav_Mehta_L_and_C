using ProductManagement.Entities;

namespace ProductManagement.Services.Contracts
{
    public interface IProductService
    {
        /// <summary>
        /// Get any Product by it's Unique Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Product> GetProduct(int id);

        Task<List<Product>> GetAllProducts();
        /// <summary>
        /// Add new Product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        Task AddProduct(Product product);

        /// <summary>
        /// Delete Product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteProduct(int id);
        /// <summary>
        /// Updated Old Product
        /// </summary>
        /// <param name="updatedProduct"></param>
        /// <returns></returns>
        Task UpdateProduct(Product updatedProduct);

        /// <summary>
        /// To Find Existing Product with Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool ProductExists(string name);
    }
}
