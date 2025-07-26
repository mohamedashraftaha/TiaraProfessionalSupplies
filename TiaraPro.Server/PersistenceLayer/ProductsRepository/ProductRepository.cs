using Microsoft.EntityFrameworkCore;
using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.ProductsRepository;

public class ProductRepository : IProductRepository
{
    private readonly TiaraDbContext _context;
    public ProductRepository(TiaraDbContext context)
    {
        _context = context;
    }
    public async Task<List<Product>> GetAllProductsAsync()
    {
        try
        {
            return await _context.Products.OrderBy(p => p.Name).ToListAsync();

        }
        catch (Exception ex)
        {
            Console.Write("An Error Has Occured {0}", ex);
            return new List<Product>();

        }
    }

    public async Task<List<Product>> GetAllProductsWithVariantsAsync()
    {
        try
        {
            return await _context.Products.Include(p => p.VariantProducts).OrderBy(p => p.Name).ToListAsync();

        }
        catch (Exception ex)
        {
            Console.Write("An Error Has Occured {0}", ex);
            return new List<Product>();

        }
    }
    public async Task<Product> GetProductByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.VariantProducts)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Product>> GetProductsByCategoryIdAsync(int categoryId)
    {
        return await _context.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
    }
    public async Task<Product> AddProductAsync(Product product)
    {
        if (string.IsNullOrEmpty(product.LogoUrl))
        {
            product.LogoUrl = null;
        }
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product;
    }
    public async Task<Product> UpdateProductAsync(Product product)
    {

        if (string.IsNullOrEmpty(product.LogoUrl))
        {
            product.LogoUrl = null;
        }
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }
    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await GetProductByIdAsync(id);
        if (product == null) return false;
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> ProductExistsAsync(int id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }
    public async Task<bool> ProductNameExistsAsync(string name)
    {
        return await _context.Products.AnyAsync(p => p.Name == name);
    }
}
