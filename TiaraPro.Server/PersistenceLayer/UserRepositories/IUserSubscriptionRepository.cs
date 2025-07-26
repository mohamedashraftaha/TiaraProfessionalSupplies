using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.UserRepositories
{
    public interface IUserSubscriptionRepository
    {
        Task<UserSubscription?> GetActiveUserSubscriptionAsync(int userId);
        Task<UserSubscription?> GetByOrderIdAsync(int orderId);
        Task<List<UserSubscription>> GetUserSubscriptionsAsync(int userId);

        Task<List<UserSubscription?>?> GetAllUsersSubscriptionsAsync();
        Task CreateAsync(UserSubscription userSubscription);
        Task UpdateAsync(UserSubscription userSubscription);
        Task DeactivateUserSubscriptionsAsync(int userId);
    }
}