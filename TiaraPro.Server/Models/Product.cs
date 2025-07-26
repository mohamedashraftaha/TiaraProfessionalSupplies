using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
namespace TiaraPro.Server.Models
{
    [Index(nameof(SKU), IsUnique = true)]
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;
        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = null!;
        public int Quantity { get; set; }
        [Precision(18, 2)]
        public decimal Price { get; set; }
        [StringLength(2000)]
        public string? Description { get; set; }

        [JsonProperty("isActive")]
        public bool? IsActive { get; set; } = true;

        [JsonProperty("categoryId")]
        public int? CategoryId { get; set; }

        [JsonProperty("isVariant")]
        public bool? IsVariant { get; set; } = false;
        [JsonProperty("productId")]
        public int? TiaraProductId { get; set; }


        [StringLength(50)]
        public string? ParentSKU { get; set; }
        [StringLength(100)]
        public string? Brand { get; set; }
        [StringLength(100)]

        [JsonProperty("productLine")]
        public string? ProductLine { get; set; }
        [StringLength(500)]

        [JsonProperty("logoUrl")]
        public string? LogoUrl { get; set; }

        // Navigation property for variant products

        public ICollection<ProductVariant>? VariantProducts { get; set; }

    }
}