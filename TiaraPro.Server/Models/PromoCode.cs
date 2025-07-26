using System;
using System.ComponentModel.DataAnnotations;

namespace TiaraPro.Server.Models
{
    public class PromoCode
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [Required]
        public decimal DiscountAmount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public int? MaxUses { get; set; } = 100000;

        public int CurrentUses { get; set; }

        public decimal? MinimumOrderAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
} 