using TiaraPro.Server.Models;
using TiaraPro.Server.PersistenceLayer.UnitOfWork;
using TiaraPro.Server.Services.UsersService;

namespace TiaraPro.Server.Services.TiaraAI
{
    public class TiaraAISubscriptionService : ITiaraAISubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TiaraAISubscriptionService> _logger;
        private readonly IUserService _userService;

        public TiaraAISubscriptionService(IUnitOfWork unitOfWork, ILogger<TiaraAISubscriptionService> logger, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userService = userService;
        }

        public async Task<List<TiaraAISubscription>> GetAvailableSubscriptionsAsync()
        {
            try
            {
                return await _unitOfWork.TiaraAISubscriptions.GetActiveSubscriptionsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available subscriptions");
                return new List<TiaraAISubscription>();
            }
        }

        public async Task<TiaraAISubscription?> GetSubscriptionByIdAsync(int subscriptionId)
        {
            try
            {
                return await _unitOfWork.TiaraAISubscriptions.GetByIdAsync(subscriptionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving subscription by ID: {SubscriptionId}", subscriptionId);
                return null;
            }
        }

        public async Task<List<UserSubscription>?>? GetActiveUserSubscriptionsAsync()
        {
            try
            {
                return await _unitOfWork.UserSubscriptions.GetAllUsersSubscriptionsAsync();

            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return new List<UserSubscription>();
            }
        }

        public async Task<bool> CreateUserSubscriptionAsync(int userId, int subscriptionId, int orderId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var subscription = await GetSubscriptionByIdAsync(subscriptionId);
                if (subscription == null)
                {
                    _logger.LogWarning("Subscription not found: {SubscriptionId}", subscriptionId);
                    return false;
                }

                var existingUserSubscription = await _unitOfWork.UserSubscriptions.GetActiveUserSubscriptionAsync(userId);
                if (existingUserSubscription != null)
                {
                    _logger.LogWarning("User already has an active subscription: {UserId}", userId);
                    if (existingUserSubscription.IsActive)
                    {
                        return false; // User already has an active subscription
                    }
                }

                var userSubscription = new UserSubscription
                {
                    UserId = userId,
                    SubscriptionId = subscriptionId,
                    OrderId = orderId,
                    SegmentationsAllowed = subscription.SegmentationsAllowed,
                    SegmentationsUsed = 0,
                    IsActive = false // Will be activated after payment confirmation
                };

                await _unitOfWork.UserSubscriptions.CreateAsync(userSubscription);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error creating user subscription");
                return false;
            }
        }

        public async Task<UserSubscription?> GetActiveUserSubscriptionAsync(int userId)
        {
            try
            {
                return await _unitOfWork.UserSubscriptions.GetActiveUserSubscriptionAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active user subscription for user: {UserId}", userId);
                return null;
            }
        }

        public async Task<bool> UpdateUserSubscriptionUsageByEmailAsync(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("User not found for email: {Email}", email);
                    return false;
                }
                var userSubscription = await GetActiveUserSubscriptionAsync(user.Id);
                if (userSubscription == null)
                {
                    _logger.LogWarning("No active subscription found for user: {UserId}", user.Id);
                    return false;
                }
                // Update the segmentations used count
                userSubscription.SegmentationsUsed++;
                await _unitOfWork.UserSubscriptions.UpdateAsync(userSubscription);
                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user subscription usage for email: {Email}", email);
                return false;
            }
        }

        public async Task<bool> ActivateUserSubscriptionAsync(int userId, int orderId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Get the user subscription for this order
                var userSubscription = await _unitOfWork.UserSubscriptions.GetByOrderIdAsync(orderId);
                if (userSubscription == null || userSubscription.UserId != userId)
                {
                    _logger.LogWarning("User subscription not found for order: {OrderId}", orderId);
                    return false;
                }

                // Deactivate any existing active subscriptions for this user
                await _unitOfWork.UserSubscriptions.DeactivateUserSubscriptionsAsync(userId);

                // Activate the new subscription
                userSubscription.IsActive = true;
                await _unitOfWork.UserSubscriptions.UpdateAsync(userSubscription);

                // Update user's TiaraAiActive flag
                var user = await _userService.GetUserByIdAsync(userId);
                if (user != null)
                {
                    user.TiaraAiActive = true;
                    await _unitOfWork.Users.Update(user);
                }

                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error activating user subscription");
                return false;
            }
        }

        public async Task<bool> SeedDefaultSubscriptionsAsync()
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var existingSubscriptions = await GetAvailableSubscriptionsAsync();
                if (existingSubscriptions.Any())
                {
                    return true; // Already seeded
                }

                var subscriptions = new List<TiaraAISubscription>
                {
                    new TiaraAISubscription
                    {
                        Name = "Light Package",
                        Price = 2900,
                        SegmentationsAllowed = 10,
                        Description = "Perfect for small practices - 10 AI segmentations",
                        IsActive = true
                    },
                    new TiaraAISubscription
                    {
                        Name = "Pro",
                        Price = 6100,
                        SegmentationsAllowed = 25,
                        Description = "Ideal for growing practices - 25 AI segmentations",
                        IsActive = true
                    },
                    new TiaraAISubscription
                    {
                        Name = "Premium",
                        Price = 9800,
                        SegmentationsAllowed = 50,
                        Description = "For busy practices - 50 AI segmentations",
                        IsActive = true
                    }
                };

                foreach (var subscription in subscriptions)
                {
                    await _unitOfWork.TiaraAISubscriptions.CreateAsync(subscription);
                }

                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.LogError(ex, "Error seeding default subscriptions");
                return false;
            }
        }
    }
}