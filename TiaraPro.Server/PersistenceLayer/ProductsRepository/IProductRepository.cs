using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.ProductsRepository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync();

        Task<List<Product>> GetAllProductsWithVariantsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<List<Product>> GetProductsByCategoryIdAsync(int categoryId);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<bool> ProductExistsAsync(int id);
        Task<bool> ProductNameExistsAsync(string name);
    }
}
