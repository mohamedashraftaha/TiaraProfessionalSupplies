using Microsoft.AspNetCore.Mvc;
using TiaraPro.Server.Models;
using TiaraPro.Server.Services.Notifications;

namespace TiaraPro.Server.Controllers;

[Route("api/[controller]")]
[ApiController]

public class NotificationsController : ControllerBase
{
    private readonly INotificationsService _notificationsService;

    private readonly ILogger<NotificationsController> _logger;
    public NotificationsController(INotificationsService notificationsService, ILogger<NotificationsController> logger)
    {
        _notificationsService = notificationsService;
        _logger = logger;
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotificationById(int id)
    {
        try
        {
            var notification = await _notificationsService.GetNotificationByIdAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            return Ok(notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notification with ID: {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
    [HttpGet]
    public async Task<IActionResult> GetAllNotifications()
    {
        try
        {
            var notifications = await _notificationsService.GetAllNotificationsAsync();
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all notifications.");
            return StatusCode(500, "Internal server error");
        }
    }
    [HttpPost]
    public async Task<IActionResult> AddNotification([FromBody] Notification notification)
    {
        if (notification == null)
        {
            return BadRequest("Notification cannot be null");
        }
        try
        {
            await _notificationsService.AddNotificationAsync(notification);
            return CreatedAtAction(nameof(GetNotificationById), new { id = notification.Id }, notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding notification.");
            return StatusCode(500, "Internal server error");
        }
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNotification(int id, [FromBody] Notification notification)
    {
        if (notification == null || notification.Id != id)
        {
            return BadRequest("Notification cannot be null and ID must match");
        }
        try
        {
            var existingNotification = await _notificationsService.GetNotificationByIdAsync(id);
            if (existingNotification == null)
            {
                return NotFound();
            }
            await _notificationsService.UpdateNotificationAsync(notification);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating notification with ID: {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNotification(int id)
    {
        try
        {
            var existingNotification = await _notificationsService.GetNotificationByIdAsync(id);
            if (existingNotification == null)
            {
                return NotFound();
            }
            await _notificationsService.DeleteNotificationAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting notification with ID: {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserNotifications(int userId)
    {
        try
        {
            var userNotifications = await _notificationsService.GetUserNotificationsAsync(userId);
            return Ok(userNotifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving user notifications for user {userId}");
            return StatusCode(500, "Internal server error");
        }
    }
    [HttpPost("user-notification")]
    public async Task<IActionResult> AddUserNotification([FromBody] UserNotification userNotification)
    {
        if (userNotification == null)
            return BadRequest("UserNotification cannot be null");
        try
        {
            await _notificationsService.AddUserNotificationAsync(userNotification);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user notification.");
            return StatusCode(500, "Internal server error");
        }
    }
    [HttpPost("user-notification/{userNotificationId}/read")]
    public async Task<IActionResult> MarkUserNotificationAsRead(int userNotificationId)
    {
        try
        {
            await _notificationsService.MarkUserNotificationAsReadAsync(userNotificationId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error marking user notification {userNotificationId} as read");
            return StatusCode(500, "Internal server error");
        }
    }
    [HttpPost("user-notification/{userNotificationId}/unread")]
    public async Task<IActionResult> MarkUserNotificationAsUnread(int userNotificationId)
    {
        try
        {
            await _notificationsService.MarkUserNotificationAsUnreadAsync(userNotificationId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error marking user notification {userNotificationId} as unread");
            return StatusCode(500, "Internal server error");
        }
    }
}
