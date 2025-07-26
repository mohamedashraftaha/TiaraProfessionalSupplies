using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.CategoriesRepository
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);

        Task<List<Category>> GetAllSubCategories(int id);

        Task<List<Product>> GetCategoryProducts(int id);
        Task<Category> AddCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
