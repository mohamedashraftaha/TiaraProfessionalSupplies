using Microsoft.EntityFrameworkCore;
using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.UserRepositories
{
    public class UserSubscriptionRepository : IUserSubscriptionRepository
    {
        private readonly TiaraDbContext _context;

        public UserSubscriptionRepository(TiaraDbContext context)
        {
            _context = context;
        }

        public async Task<UserSubscription?> GetActiveUserSubscriptionAsync(int userId)
        {
            return await _context.UserSubscriptions
                .Include(us => us.Subscription)
                .Include(us => us.Order)
                .FirstOrDefaultAsync(us => us.UserId == userId && us.IsActive);
        }

        public async Task<List<UserSubscription?>?> GetAllUsersSubscriptionsAsync()
        {
            return await _context.UserSubscriptions
                .Include(us => us.Subscription)
                .Include(us => us.User)
                .ToListAsync();
        }

        public async Task<UserSubscription?> GetByOrderIdAsync(int orderId)
        {
            return await _context.UserSubscriptions
                .Include(us => us.Subscription)
                .Include(us => us.User)
                .FirstOrDefaultAsync(us => us.OrderId == orderId);
        }

        public async Task<List<UserSubscription>> GetUserSubscriptionsAsync(int userId)
        {
            return await _context.UserSubscriptions
                .Include(us => us.Subscription)
                .Include(us => us.Order)
                .Where(us => us.UserId == userId)
                .OrderByDescending(us => us.SubscribedAt)
                .ToListAsync();
        }

        public async Task CreateAsync(UserSubscription userSubscription)
        {
            await _context.UserSubscriptions.AddAsync(userSubscription);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserSubscription userSubscription)
        {
            _context.UserSubscriptions.Update(userSubscription);
            await _context.SaveChangesAsync();
        }

        public async Task DeactivateUserSubscriptionsAsync(int userId)
        {
            var activeSubscriptions = await _context.UserSubscriptions
                .Where(us => us.UserId == userId && us.IsActive)
                .ToListAsync();

            foreach (var subscription in activeSubscriptions)
            {
                subscription.IsActive = false;
            }

            await _context.SaveChangesAsync();
        }
    }
}