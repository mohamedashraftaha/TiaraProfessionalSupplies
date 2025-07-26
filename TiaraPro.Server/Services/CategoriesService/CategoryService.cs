using TiaraPro.Server.Models;
using TiaraPro.Server.PersistenceLayer.UnitOfWork;

namespace TiaraPro.Server.Services.CategoriesService;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CategoryService> _logger;
    public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        try
        {
            return await _unitOfWork.Categories.GetAllCategoriesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories.");
            throw;
        }
    }

    public async Task<List<Category>> GetAllSubCategories(int id)
    {
        try
        {
            return await _unitOfWork.Categories.GetAllSubCategories(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving subcategories.");
            throw;
        }
    }
    public async Task<Category> GetCategoryByIdAsync(int id)
    {
        try
        {
            return await _unitOfWork.Categories.GetCategoryByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category by ID.");
            throw;
        }
    }
    public async Task<Category> AddCategoryAsync(Category category)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var cat = await _unitOfWork.Categories.AddCategoryAsync(category);
            await _unitOfWork.CompleteAsync();
            return cat;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error adding category.");
            throw;
        }
    }
    public async Task<Category> UpdateCategoryAsync(Category category)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            var updatedCategory = await _unitOfWork.Categories.UpdateCategoryAsync(category);
            await _unitOfWork.CompleteAsync();
            return updatedCategory;

        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error updating category.");
            throw;
        }
    }
    public async Task<bool> DeleteCategoryAsync(int id)
    {
        try
        {
            return await _unitOfWork.Categories.DeleteCategoryAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category.");
            throw;
        }
    }
    public async Task<bool> CategoryExistsAsync(int id)
    {
        try
        {
            var category = await _unitOfWork.Categories.GetCategoryByIdAsync(id);
            return category != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if category exists.");
            throw;
        }
    }
    public async Task<bool> CategoryNameExistsAsync(string name)
    {
        try
        {
            var categories = await _unitOfWork.Categories.GetAllCategoriesAsync();
            return categories.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if category name exists.");
            throw;
        }
    }

    public async Task<List<Product>> GetCategoryProducts(int id)
    {
        try
        {
            return await _unitOfWork.Categories.GetCategoryProducts(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category products.");
            throw;
        }
    }

}
