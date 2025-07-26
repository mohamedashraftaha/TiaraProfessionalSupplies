using Microsoft.EntityFrameworkCore;
using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.TiaraAI
{
    public class TiaraAISubscriptionRepository : ITiaraAISubscriptionRepository
    {
        private readonly TiaraDbContext _context;

        public TiaraAISubscriptionRepository(TiaraDbContext context)
        {
            _context = context;
        }

        public async Task<List<TiaraAISubscription>> GetActiveSubscriptionsAsync()
        {
            return await _context.TiaraAISubscriptions
                .Where(s => s.IsActive)
                .OrderBy(s => s.Price)
                .ToListAsync();
        }

        public async Task<TiaraAISubscription?> GetByIdAsync(int id)
        {
            return await _context.TiaraAISubscriptions
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task CreateAsync(TiaraAISubscription subscription)
        {
            await _context.TiaraAISubscriptions.AddAsync(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TiaraAISubscription subscription)
        {
            subscription.UpdatedAt = DateTimeOffset.UtcNow;
            _context.TiaraAISubscriptions.Update(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var subscription = await GetByIdAsync(id);
            if (subscription != null)
            {
                _context.TiaraAISubscriptions.Remove(subscription);
                await _context.SaveChangesAsync();
            }
        }
    }
}