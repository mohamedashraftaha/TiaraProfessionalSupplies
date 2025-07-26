using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TiaraPro.Server.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }


        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string PaymentStatus { get; set; } = "Pending";

        [Precision(18, 2)]
        public decimal Amount { get; set; }

        [StringLength(100)]
        public string? TransactionId { get; set; }

        [StringLength(100)]
        public string? CardholderName { get; set; }

        [StringLength(4)]
        public string? CardLastFour { get; set; }

        [StringLength(20)]
        public string? CardBrand { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public string? Notes { get; set; }
    }
}
