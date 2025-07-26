using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.Notifications;

public interface INotificationRepository
{
    Task<Notification?> GetNotificationByIdAsync(int id);
    Task<IEnumerable<Notification>> GetAllNotificationsAsync();
    Task<int> AddNotificationAsync(Notification notification);
    Task UpdateNotificationAsync(Notification notification);
    Task DeleteNotificationAsync(int id);
    Task SaveChangesAsync();
    Task<IEnumerable<Notification>> GetNotificationsForUserAsync(int userId);
    Task MarkAsReadAsync(int notificationId, int userId);
    Task MarkAsUnreadAsync(int notificationId, int userId);
    Task AddUserNotificationAsync(UserNotification userNotification);
    Task<IEnumerable<UserNotification>> GetUserNotificationsAsync(int userId);
    Task MarkUserNotificationAsReadAsync(int userNotificationId);
    Task MarkUserNotificationAsUnreadAsync(int userNotificationId);
}
