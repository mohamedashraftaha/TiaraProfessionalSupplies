using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TiaraPro.Server.Models;
public class Notification
{
    public int Id { get; set; }                     
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationCategory Category { get; set; }
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? UserId { get; set; } // Nullable for global notifications
    public bool IsRead { get; set; } = false;
    public User? User { get; set; }
}

public enum NotificationCategory
{
    NewProduct,       
    NewFeature,       
    NewEvent,         
    NewTraining,      
    TiaraAIUpdates    
}
