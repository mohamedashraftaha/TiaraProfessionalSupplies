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
    public async Task AddProductAsync(Product product)
    {
        if (string.IsNullOrEmpty(product.LogoUrl))
        {
            product.LogoUrl = null;
        }
        await _context.Products.AddAsync(product);
    }
    public async Task UpdateProductAsync(Product product)
    {

        if (string.IsNullOrEmpty(product.LogoUrl))
        {
            product.LogoUrl = null;
        }
        _context.Products.Update(product);
    }
    public async Task DeleteProductAsync(int id)
    {
        var product = await GetProductByIdAsync(id);
        _context.Products.Remove(product);
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
