using System.ComponentModel.DataAnnotations;

namespace TiaraPro.Server.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;
        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = null!;
        [StringLength(500)]
        public string? Speakers { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [StringLength(300)]
        public string? Location { get; set; }
        [StringLength(500)]
        public string? ImageUrl { get; set; }
        public int? Capacity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
} 