using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.TiaraAI
{
    public interface ITiaraAISubscriptionRepository
    {
        Task<List<TiaraAISubscription>> GetActiveSubscriptionsAsync();
        Task<TiaraAISubscription?> GetByIdAsync(int id);
        Task CreateAsync(TiaraAISubscription subscription);
        Task UpdateAsync(TiaraAISubscription subscription);
        Task DeleteAsync(int id);
    }
}