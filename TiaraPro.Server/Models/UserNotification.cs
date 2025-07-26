using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiaraPro.Server.Models;

public class UserNotification
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; }
    public int NotificationId { get; set; }
    public bool IsRead { get; set; } = false;

    [ForeignKey("UserId")]
    public User? User { get; set; }
    [ForeignKey("NotificationId")]
    public Notification? Notification { get; set; }
} 