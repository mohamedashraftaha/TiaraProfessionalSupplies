using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;
using TiaraPro.Server.Models;
using TiaraPro.Server.Services.CategoriesService;

namespace TiaraPro.Server.Controllers;
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    [HttpGet("{id}/subcategories")]
    public async Task<IActionResult> GetAllSubCategories(int id)
    {
        var subCategories = await _categoryService.GetAllSubCategories(id);
        if (subCategories == null || !subCategories.Any())
        {
            return NotFound();
        }
        return Ok(subCategories);
    }

    [HttpGet("{id}/products")]
    public async Task<IActionResult> GetCategoryProducts(int id)
    {
        var products = await _categoryService.GetCategoryProducts(id);
        if (products == null)
        {
            return NotFound();
        }
        return Ok(products);
    }
    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] Category category)
    {
        if (ModelState.IsValid)
        {
            var createdCategory = await _categoryService.AddCategoryAsync(category);
            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
        }
        return BadRequest(ModelState);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
    {
        if (id != category.Id || !ModelState.IsValid)
        {
            return BadRequest();
        }

        var updatedCategory = await _categoryService.UpdateCategoryAsync(category);
        if (updatedCategory == null)
        {
            return NotFound();
        }

        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
