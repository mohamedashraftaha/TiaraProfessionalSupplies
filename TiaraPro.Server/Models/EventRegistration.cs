using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiaraPro.Server.Models
{
    public class EventRegistration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [Required]
        public int EventId { get; set; }
        [ForeignKey("EventId")]
        public Event Event { get; set; } = null!;

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    }
} 