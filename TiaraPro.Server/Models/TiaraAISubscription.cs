using System.ComponentModel.DataAnnotations;

namespace TiaraPro.Server.Models
{
    public class TiaraAISubscription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int SegmentationsAllowed { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
} 