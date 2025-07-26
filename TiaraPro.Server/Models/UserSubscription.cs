using System.ComponentModel.DataAnnotations;

namespace TiaraPro.Server.Models
{
    public class UserSubscription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int SubscriptionId { get; set; }

        [Required]
        public int OrderId { get; set; }

        public int SegmentationsUsed { get; set; } = 0;

        public int SegmentationsAllowed { get; set; }

        public DateTimeOffset SubscribedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? ExpiresAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public User? User { get; set; }
        public TiaraAISubscription? Subscription { get; set; }
        public Order? Order { get; set; }
    }
} 