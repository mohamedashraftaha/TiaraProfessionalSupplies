using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TiaraPro.Server.Models;
using TiaraPro.Server.Services.ProductsService;
using TiaraPro.Server.Services.AwsS3;

namespace TiaraPro.Server.Controllers;

[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IAWSS3Service _awsS3Service;
    public ProductController(IProductService productService, IAWSS3Service awsS3Service)
    {
        _productService = productService;
        _awsS3Service = awsS3Service;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }


    [HttpGet("withVariants")]
    public async Task<IActionResult> GetAllProductsWithVariants()
    {
        var products = await _productService.GetAllProductsWithVariantsAsync();
        return Ok(products);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpGet("variants/{id}")]
    public async Task<IActionResult> GetProductsVariantsById(int id)
    {
        var variants = await _productService.GetAllVariantsAsync(id);
        if (variants == null || !variants.Any())
        {
            return NotFound();
        }
        return Ok(variants);


    }

    [HttpGet("variant/{id}/{side}/{size}/{option}")]
    public async Task<IActionResult> GetProductVariantById(int id, int? side, int? size, int? option)
    {
        ProductVariant? variants = null;
        if (side != null && size != null)
        {
            variants = await _productService.GetProductVariantAsync(id, side.Value, size.Value);
        }
        if (side == null && size != null)
        {
            variants = await _productService.GetProductBySizeAsync(id, size.Value);
        }
        if (side == null && size == null && option != null)
        {
            variants = await _productService.GetProductByOptionAsync(id, option.Value);
        }
        if (variants == null)
        {
            return NotFound();
        }

        return Ok(variants);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] Product product)
    {
        if (ModelState.IsValid)
        {
            var createdProduct = await _productService.AddProductAsync(product);
            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
        }
        return BadRequest(ModelState);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
    {
        if (id != product.Id || !ModelState.IsValid)
        {
            return BadRequest();
        }
        var updatedProduct = await _productService.UpdateProductAsync(product);
        if (updatedProduct == null)
        {
            return NotFound();
        }
        return Ok(updatedProduct);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var deleted = await _productService.DeleteProductAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        return Ok();
    }
    //[HttpPut("update-stock/{productId}/{quantity}")]
    //public async Task<IActionResult> UpdateProductStockLevel(int productId, int quantity)
    //{
    //    var updated = await _productService.UpdateProductStockLevel(productId, quantity);
    //    if (!updated)
    //    {
    //        return NotFound();
    //    }
    //    return NoContent();
    //}

    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");


        var allowedTypes = new[] { "image/jpeg", "image/png", "image/jpg", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            return BadRequest("Invalid file type. Only image files are allowed.");

        try
        {
            var publicUrl = await _awsS3Service.UploadImage(file);
            if (publicUrl == null)
                return BadRequest("Failed to upload image");
            return Ok(new { url = publicUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error uploading image: {ex.Message}");
        }
    }

    [HttpPut("variant/{id}")]
    public async Task<IActionResult> UpdateProductVariant(int id, [FromBody] ProductVariant variant)
    {
        if (id != variant.Id || !ModelState.IsValid)
        {
            return BadRequest();
        }
        var updatedVariant = await _productService.UpdateProductVariantAsync(variant);
        if (updatedVariant == null)
        {
            return NotFound();
        }
        return Ok(updatedVariant);
    }
}
