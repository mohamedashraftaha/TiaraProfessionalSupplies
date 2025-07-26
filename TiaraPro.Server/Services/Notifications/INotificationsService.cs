using TiaraPro.Server.Models;

namespace TiaraPro.Server.Services.Notifications;

public interface INotificationsService
{
    Task<Notification?> GetNotificationByIdAsync(int id);
    Task<IEnumerable<Notification>> GetAllNotificationsAsync();
    Task<bool> AddNotificationAsync(Notification notification);
    Task UpdateNotificationAsync(Notification notification);
    Task DeleteNotificationAsync(int id);
    Task<IEnumerable<Notification>> GetNotificationsForUserAsync(int userId);
    Task MarkAsReadAsync(int notificationId, int userId);
    Task MarkAsUnreadAsync(int notificationId, int userId);
    Task AddUserNotificationAsync(UserNotification userNotification);
    Task<IEnumerable<UserNotification>> GetUserNotificationsAsync(int userId);
    Task MarkUserNotificationAsReadAsync(int userNotificationId);
    Task MarkUserNotificationAsUnreadAsync(int userNotificationId);
}
