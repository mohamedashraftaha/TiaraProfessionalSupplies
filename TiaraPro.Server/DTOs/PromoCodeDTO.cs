using System;

namespace TiaraPro.Server.DTOs
{
    public class ValidatePromoCodeRequest
    {
        public string Code { get; set; }
        public decimal OrderAmount { get; set; }
        public int UserId { get; set; }
    }

    public class ValidatePromoCodeResponse
    {
        public int PromoCodeId { get; set; }
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
    }

    public class PromoCodeResponse
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public int? MaxUses { get; set; }
        public int CurrentUses { get; set; }
        public decimal? MinimumOrderAmount { get; set; }
    }
} 