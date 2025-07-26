using Microsoft.EntityFrameworkCore;
using TiaraPro.Server.Models;

namespace TiaraPro.Server.PersistenceLayer.Notifications;

public class NotificationRepository : INotificationRepository
{
    private readonly TiaraDbContext _context;
    public NotificationRepository(TiaraDbContext context)
    {
        _context = context;
    }
    public async Task<Notification?> GetNotificationByIdAsync(int id)
    {
        return await _context.Notifications.FindAsync(id);
    }
    public async Task<IEnumerable<Notification>> GetAllNotificationsAsync()
    {
        return await _context.Notifications.ToListAsync();
    }
    public async Task<int> AddNotificationAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        int rowsAffected = await _context.SaveChangesAsync();

        return rowsAffected;
    }
    public async Task UpdateNotificationAsync(Notification notification)
    {
        _context.Notifications.Update(notification);
        await SaveChangesAsync();
    }
    public async Task DeleteNotificationAsync(int id)
    {
        var notification = await GetNotificationByIdAsync(id);
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            await SaveChangesAsync();
        }
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    public async Task<IEnumerable<Notification>> GetNotificationsForUserAsync(int userId)
    {
        return await _context.Notifications.Where(n => n.UserId == userId || n.UserId == null).ToListAsync();
    }
    public async Task MarkAsReadAsync(int notificationId, int userId)
    {
        var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);
        if (notification != null)
        {
            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }
    public async Task MarkAsUnreadAsync(int notificationId, int userId)
    {
        var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);
        if (notification != null)
        {
            notification.IsRead = false;
            await _context.SaveChangesAsync();
        }
    }
    public async Task AddUserNotificationAsync(UserNotification userNotification)
    {
        await _context.UserNotifications.AddAsync(userNotification);
        await _context.SaveChangesAsync();
    }
    public async Task<IEnumerable<UserNotification>> GetUserNotificationsAsync(int userId)
    {
        return await _context.UserNotifications
            .Include(un => un.Notification)
            .Where(un => un.UserId == userId)
            .ToListAsync();
    }
    public async Task MarkUserNotificationAsReadAsync(int userNotificationId)
    {
        var userNotification = await _context.UserNotifications.FindAsync(userNotificationId);
        if (userNotification != null)
        {
            userNotification.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }
    public async Task MarkUserNotificationAsUnreadAsync(int userNotificationId)
    {
        var userNotification = await _context.UserNotifications.FindAsync(userNotificationId);
        if (userNotification != null)
        {
            userNotification.IsRead = false;
            await _context.SaveChangesAsync();
        }
    }
}
