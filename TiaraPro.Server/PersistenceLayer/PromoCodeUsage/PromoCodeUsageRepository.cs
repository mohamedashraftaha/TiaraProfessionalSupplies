using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.PromoCodeUsage;

public class PromoCodeUsageRepository : IPromoCodeUsageRepository
{
    private readonly TiaraDbContext _context;
    public PromoCodeUsageRepository(TiaraDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddPromoCodeUsageAsync(UserPromoCodeUsage promoCodeUsage)
    {
        try
        {
            await _context.UserPromoCodeUsages.AddAsync(promoCodeUsage);
            await _context.SaveChangesAsync();
            return true;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while adding promo code usage: {ex.Message}");
            return false;
        }
    }

}
