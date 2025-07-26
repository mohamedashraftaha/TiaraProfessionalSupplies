using TiaraPro.Server.Models;

namespace TiaraPro.Server.Services.CategoriesService;

public interface ICategoryService
{
    Task<List<Category>> GetAllCategoriesAsync();
    Task<Category> GetCategoryByIdAsync(int id);

    Task<List<Category>> GetAllSubCategories(int id);

    Task<List<Product>> GetCategoryProducts(int id);
    Task<Category> AddCategoryAsync(Category category);
    Task<Category> UpdateCategoryAsync(Category category);
    Task<bool> DeleteCategoryAsync(int id);
    Task<bool> CategoryExistsAsync(int id);
    Task<bool> CategoryNameExistsAsync(string name);

}
