using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.ProductVariants
{
    public interface IProductVariantsRepository
    {
        Task<List<ProductVariant>> GetAllVariantsAsync(int productId);

        Task<ProductVariant> GetVariant(int productId, int side, int size);

        Task<bool> UpdateProductVariantStockLevel(int productId, int side, int size, int quantity);

        Task<ProductVariant> UpdateProductVariantAsync(ProductVariant productVariant);

        Task<ProductVariant> AddProductVariantAsync(ProductVariant variant);

        Task<ProductVariant?> GetProductBySizeAsync(int productId, int size);
        Task<ProductVariant?> GetProductByOptionAsync(int productId, int option);

    }
}
