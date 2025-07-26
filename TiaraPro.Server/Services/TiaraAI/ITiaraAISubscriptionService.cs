using TiaraPro.Server.Models;

namespace TiaraPro.Server.Services.TiaraAI
{
    public interface ITiaraAISubscriptionService
    {
        Task<List<TiaraAISubscription>> GetAvailableSubscriptionsAsync();
        Task<TiaraAISubscription?> GetSubscriptionByIdAsync(int subscriptionId);
        Task<bool> CreateUserSubscriptionAsync(int userId, int subscriptionId, int orderId);
        Task<UserSubscription?> GetActiveUserSubscriptionAsync(int userId);
        Task<bool> ActivateUserSubscriptionAsync(int userId, int orderId);
        Task<bool> SeedDefaultSubscriptionsAsync();
        Task<bool> UpdateUserSubscriptionUsageByEmailAsync(string email);

        Task<List<UserSubscription?>?> GetActiveUserSubscriptionsAsync();
    }
}