using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiaraPro.Server.Models;

public class UserPromoCode
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int PromoCodeId { get; set; }

    [Required]
    public DateTime UsedAt { get; set; } = DateTime.UtcNow;

    public int? OrderId { get; set; }
}
