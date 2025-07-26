using Microsoft.EntityFrameworkCore;
using TiaraPro.Server.Migrations;
using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.ProductVariants;

public class ProductVariantsRepository : IProductVariantsRepository
{

    private readonly TiaraDbContext _context;
    public ProductVariantsRepository(TiaraDbContext context)
    {
        _context = context;
    }
    public async Task<List<ProductVariant>> GetAllVariantsAsync(int productId)
    {
        try
        {
            return await _context.ProductVariants
                .Where(v => v.ProductId == productId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Console.Write("An Error Has Occured {0}", ex);
            return new List<ProductVariant>();

        }
    }

    public async Task<ProductVariant?> GetProductBySizeAsync(int productId, int size)
    {
        try
        {
            return await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.ProductId == productId && v.Size == size);
        }
        catch (Exception ex)
        {
            Console.Write("An Error Has Occured {0}", ex);
            return null;
        }
    }

    public async Task<ProductVariant?> GetProductByOptionAsync(int productId, int option)
    {
        try
        {
            return await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.ProductId == productId && (int)v.VariableOption == option);
        }
        catch (Exception ex)
        {
            Console.Write("An Error Has Occured {0}", ex);
            return null;
        }
    }


    public async Task<ProductVariant> GetVariant(int productId, int side, int size)
    {
        try
        {
            return await _context.ProductVariants
                .FirstOrDefaultAsync(v => v.ProductId == productId && (int)v.Side == side && v.Size == size);
        }
        catch (Exception ex)
        {
            Console.Write("An Error Has Occured {0}", ex);
            return null;
        }
    }

    public async Task<bool> UpdateProductVariantStockLevel(int productId, int side, int size, int quantity)
    {
        try
        {
            var variant = await GetVariant(productId, side, size);
            if (variant == null) return false;
            variant.Quantity += quantity;
            _context.ProductVariants.Update(variant);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.Write("An Error Has Occured {0}", ex);
            return false;
        }
    }

    public async Task<ProductVariant> UpdateProductVariantAsync(ProductVariant productVariant)
    {
        try
        {
            if (productVariant == null) return null;
            _context.ProductVariants.Update(productVariant);
            await _context.SaveChangesAsync();
            return productVariant;
        }
        catch (Exception ex)
        {
            Console.Write("An Error Has Occured {0}", ex);
            return null;
        }
    }

    public async Task<ProductVariant> AddProductVariantAsync(ProductVariant variant)
    {
        try
        {
            if (variant == null) return null;

            var product = await _context.Products.FirstOrDefaultAsync( p=> p.Id == variant.ProductId);
            if (product != null)
            {
                if (product.VariantProducts == null)
                {
                    product.VariantProducts = new List<ProductVariant>();
                }
                product.VariantProducts.Add(variant);
                product.IsVariant = true;
            }
            await _context.ProductVariants.AddAsync(variant);
            await _context.SaveChangesAsync();
            return variant;


        }
        catch (Exception ex)
        {
            return null;
        }

    }

}
