namespace TiaraPro.Server.Models;

public class UserPromoCodeUsage
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PromoCodeId { get; set; }
    public DateTime UsedAt { get; set; } = DateTime.UtcNow;
    public int? OrderId { get; set; }
}
