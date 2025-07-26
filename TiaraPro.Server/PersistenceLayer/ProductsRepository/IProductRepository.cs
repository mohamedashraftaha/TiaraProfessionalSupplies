using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.ProductsRepository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProductsAsync();

        Task<List<Product>> GetAllProductsWithVariantsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<List<Product>> GetProductsByCategoryIdAsync(int categoryId);
        Task<Product> AddProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> ProductExistsAsync(int id);
        Task<bool> ProductNameExistsAsync(string name);
    }
}
