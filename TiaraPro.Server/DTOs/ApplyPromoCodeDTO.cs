namespace TiaraPro.Server.DTOs;
    public class ApplyPromoCodeDTO
    {
        public int UserId { get; set; }
        public int PromoCodeId { get; set; }
        public int? OrderId { get; set; }
    }
