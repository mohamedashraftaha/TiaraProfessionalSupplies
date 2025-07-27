using TiaraPro.Server.Models;
using TiaraPro.Server.PersistenceLayer.UnitOfWork;

namespace TiaraPro.Server.Services.ProductsService;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _unitOfWork.Products.GetAllProductsAsync();
    }
    public async Task<List<Product>> GetAllProductsWithVariantsAsync()
    {
        return await _unitOfWork.Products.GetAllProductsWithVariantsAsync();
    }

    public async Task<ProductVariant?> GetProductBySizeAsync(int productId, int size)
    {
        return await _unitOfWork.ProductVariants.GetProductBySizeAsync(productId, size);
    }

    public async Task<ProductVariant?> GetProductByOptionAsync(int productId, int option)
    {
        return await _unitOfWork.ProductVariants.GetProductByOptionAsync(productId, option);
    }


    public async Task<Product> GetProductByIdAsync(int id)
    {
        return await _unitOfWork.Products.GetProductByIdAsync(id);
    }
    public async Task<List<Product>> GetProductsByCategoryIdAsync(int categoryId)
    {
        return await _unitOfWork.Products.GetProductsByCategoryIdAsync(categoryId);
    }
    public async Task<Product> AddProductAsync(Product product)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Products.AddProductAsync(product);
            
            int rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected == 0)
            {
                throw new Exception("No rows affected while adding product");
            }
            return product;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            throw new Exception("Error adding product", ex);
        }
    }
    public async Task<Product> UpdateProductAsync(Product product)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Products.UpdateProductAsync(product);
            int rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected == 0)
            {
                throw new Exception("No rows affected while updating product");
            }
            return product;

        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            throw new Exception("Error updating product", ex);
        }
    }

    public async Task<List<ProductVariant>> GetAllVariantsAsync(int productId)
    {
        try
        {
            return await _unitOfWork.ProductVariants.GetAllVariantsAsync(productId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving product variants", ex);
        }
    }

    public async Task<ProductVariant> GetProductVariantAsync(int productId, int side, int size)
    {
        try
        {
            return await _unitOfWork.ProductVariants.GetVariant(productId, side, size);
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving product variant", ex);
        }
    }
    public async Task<bool> DeleteProductAsync(int id)
    {
        try
        {
            var productExists = await _unitOfWork.Products.ProductExistsAsync(id);
            if (!productExists)
            {
                return false;
            }
            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Products.DeleteProductAsync(id);
            int rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected == 0)
            {
                throw new Exception("No rows affected while deleting product");
            }
            return true;

        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            throw new Exception("Error deleting product", ex);
        }
    }
    public async Task<bool> UpdateProductStockLevel(Product product, int quantity, string productName)
    {
        try
        {
            ProductVariant? variant = null;
            if (product == null)
            {
                return false;
            }
            product.Quantity += quantity;

            if (product.IsVariant == true)
            {
                int productSide = 0;
                var temp = productName.Split(',');
                if (temp.Length <= 1)
                {
                    throw new Exception("Invalid product name format for variants");
                }

                if (temp.Length > 2)
                {
                    var side = temp[1];
                    side = side.Replace(" ", "");
                    char v = temp[2].ToString()[1];
                    var size = int.Parse(v.ToString());
                    var pSide = Enum.TryParse(side, out ProductSide parsedSide);

                    variant = product.VariantProducts.FirstOrDefault(x => x.ParentProductId == product.Id && (ProductSide)x.Side == parsedSide && x.Size == size);
                }

                if (temp.Length == 2)
                {
                    if (temp[1].Length == 2)
                    {
                        char v = temp[1].ToString()[1];
                        var size = int.Parse(v.ToString());

                        variant = product.VariantProducts.FirstOrDefault(x => x.ParentProductId == product.Id && x.Size == size);

                    }
                    else
                    {
                        var option = temp[1].ToString();
                        var variableOption = Enum.TryParse(option, out VariableOptions parsedOption);
                        variant = product.VariantProducts.FirstOrDefault(x => x.ParentProductId == product.Id && (VariableOptions)x.VariableOption == parsedOption);
                    }


                }


                if (variant == null)
                {
                    throw new Exception("Product variant not found");
                }
                variant.Quantity += quantity;
                var response = await _unitOfWork.ProductVariants.UpdateProductVariantAsync(variant);
                if (response == null)
                {
                    throw new Exception("Error updating product variant stock level");
                }
            }

            await _unitOfWork.Products.UpdateProductAsync(product);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Error updating product stock level", ex);
        }
    }

    public async Task<ProductVariant?> UpdateProductVariantAsync(ProductVariant variant)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var updated = await _unitOfWork.ProductVariants.UpdateProductVariantAsync(variant);
            await _unitOfWork.CompleteAsync();
            return updated;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return null;
        }
    }

}
