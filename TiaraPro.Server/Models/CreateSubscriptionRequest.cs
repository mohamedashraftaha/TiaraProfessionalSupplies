using System.ComponentModel.DataAnnotations;

namespace TiaraPro.Server.Models
{
    public class CreateSubscriptionRequest
    {
        [Required]
        public int SubscriptionId { get; set; }

        [Required]
        public int OrderId { get; set; }
    }
} 