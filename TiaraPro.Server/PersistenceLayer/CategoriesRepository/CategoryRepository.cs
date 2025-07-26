using Microsoft.EntityFrameworkCore;
using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.CategoriesRepository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly TiaraDbContext _context;
        public CategoryRepository(TiaraDbContext context)
        {
            _context = context;
        }
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<List<Category>> GetAllSubCategories(int id)
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryId == id)
                .Include(c => c.Products)
                .ToListAsync();
        }
        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }
        public async Task<Category> AddCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }
        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await GetCategoryByIdAsync(id);
            if (category == null) return false;
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Product>> GetCategoryProducts(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null || category.Products == null)
            {
                return new List<Product>();
            }

            return category.Products.ToList();
        }
    }
}
