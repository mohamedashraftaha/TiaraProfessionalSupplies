using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.PromoCodeUsage;

public interface IPromoCodeUsageRepository
{
    Task<bool> AddPromoCodeUsageAsync(UserPromoCodeUsage promoCodeUsage);

}
