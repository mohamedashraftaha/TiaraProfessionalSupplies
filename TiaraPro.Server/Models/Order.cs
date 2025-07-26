using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiaraPro.Server.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; } 
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }

        public int? PromoCodeId { get; set; }

        [Required]
        [StringLength(200)]
        public string ShippingAddress { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string ShippingCity { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string ShippingState { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string ShippingPostalCode { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string ShippingCountry { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string ShippingPhone { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string ShippingEmail { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string ShippingUserFirstName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string ShippingUserLastName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string ShippingUserMiddleName { get; set; } = null!;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? UpdatedAt { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; } = new List<OrderItem>();

        public ICollection<Payment>? Payments { get; set; } = new List<Payment>();

        [ForeignKey("PromoCodeId")]
        public virtual PromoCode? PromoCode { get; set; }
    }
}
