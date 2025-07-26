using TiaraPro.Server.Models;

namespace TiaraPro.Server.Services.ProductsService;

public interface IProductService
{
    Task<List<Product>> GetAllProductsAsync();
    Task<List<Product>> GetAllProductsWithVariantsAsync();
    Task<Product> GetProductByIdAsync(int id);
    Task<List<Product>> GetProductsByCategoryIdAsync(int categoryId);
    Task<Product> AddProductAsync(Product product);
    Task<Product> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(int id);
    Task<bool> UpdateProductStockLevel(Product product, int quantity, string productName);

    Task<List<ProductVariant>> GetAllVariantsAsync(int productId);

    Task<ProductVariant> GetProductVariantAsync(int productId, int side, int size);

    Task<ProductVariant?> GetProductBySizeAsync(int productId, int size);

    Task<ProductVariant?> GetProductByOptionAsync(int productId, int option);

    Task<ProductVariant?> UpdateProductVariantAsync(ProductVariant variant);
}
