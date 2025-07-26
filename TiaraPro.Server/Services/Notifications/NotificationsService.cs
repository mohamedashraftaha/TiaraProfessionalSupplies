using TiaraPro.Server.Models;
using TiaraPro.Server.PersistenceLayer.UnitOfWork;
using TiaraPro.Server.Services.UsersService;

namespace TiaraPro.Server.Services.Notifications;

public class NotificationsService : INotificationsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NotificationsService> _logger;
    private readonly IUserService _userService;
    public NotificationsService(IUnitOfWork unitOfWork, ILogger<NotificationsService> logger, IUserService userService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _userService = userService;
    }
    public async Task<Notification?> GetNotificationByIdAsync(int id)
    {
        try
        {
            return await _unitOfWork.Notifications.GetNotificationByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notification with ID: {Id}", id);
            throw;
        }
    }
    public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
    {
        try
        {
            return await _unitOfWork.Notifications.GetAllNotificationsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all notifications.");
            throw;
        }
    }
    public async Task<bool> AddNotificationAsync(Notification notification)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            int rowsAffected = await _unitOfWork.Notifications.AddNotificationAsync(notification);
            if (rowsAffected > 0)
            {
                // Create UserNotification(s)
                if (notification.UserId == null)
                {
                    // Global notification: create for all users
                    var users = await _userService.GetAllUsersAsync();
                    foreach (var user in users)
                    {
                        var userNotification = new UserNotification
                        {
                            UserId = user.Id,
                            NotificationId = notification.Id,
                            IsRead = false
                        };
                        await _unitOfWork.Notifications.AddUserNotificationAsync(userNotification);
                    }
                }
                else
                {
                    // User-specific notification
                    var userNotification = new UserNotification
                    {
                        UserId = notification.UserId.Value,
                        NotificationId = notification.Id,
                        IsRead = false
                    };
                    await _unitOfWork.Notifications.AddUserNotificationAsync(userNotification);
                }
                await _unitOfWork.CompleteAsync();
                return true;
            }
            await _unitOfWork.RollbackAsync();
            _logger.LogWarning("No rows affected when creating notification. Notification ID: {NotificationId}", notification.Id);
            return false;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error creating notification for ID: {NotificationId}", notification.Id);
            return false;
        }
    }

    public async Task UpdateNotificationAsync(Notification notification)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            await _unitOfWork.Notifications.UpdateNotificationAsync(notification);
            await _unitOfWork.CompleteAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating notification with ID: {Id}", notification.Id);
            throw;
        }
    }

    public async Task DeleteNotificationAsync(int id)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            await _unitOfWork.Notifications.DeleteNotificationAsync(id);
            await _unitOfWork.CompleteAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error deleting notification with ID: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Notification>> GetNotificationsForUserAsync(int userId)
    {
        try
        {
            return await _unitOfWork.Notifications.GetNotificationsForUserAsync(userId);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();

            _logger.LogError(ex, "Error retrieving notifications for user {UserId}", userId);
            throw;
        }
    }

    public async Task MarkAsReadAsync(int notificationId, int userId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            await _unitOfWork.Notifications.MarkAsReadAsync(notificationId, userId);
            await _unitOfWork.CompleteAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();

            _logger.LogError(ex, "Error marking notification {NotificationId} as read for user {UserId}", notificationId, userId);
            throw;
        }
    }

    public async Task MarkAsUnreadAsync(int notificationId, int userId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Notifications.MarkAsUnreadAsync(notificationId, userId);
            await _unitOfWork.CompleteAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error marking notification {NotificationId} as unread for user {UserId}", notificationId, userId);
            throw;
        }
    }

    public async Task AddUserNotificationAsync(UserNotification userNotification)
    {
        await _unitOfWork.Notifications.AddUserNotificationAsync(userNotification);
    }

    public async Task<IEnumerable<UserNotification>> GetUserNotificationsAsync(int userId)
    {
        return await _unitOfWork.Notifications.GetUserNotificationsAsync(userId);
    }

    public async Task MarkUserNotificationAsReadAsync(int userNotificationId)
    {
        await _unitOfWork.Notifications.MarkUserNotificationAsReadAsync(userNotificationId);
    }

    public async Task MarkUserNotificationAsUnreadAsync(int userNotificationId)
    {
        await _unitOfWork.Notifications.MarkUserNotificationAsUnreadAsync(userNotificationId);
    }
}
