using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiaraPro.Server.Models
{
    public class DentalTrainingRegistration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        public int? OrderId { get; set; } = null;

        public bool Confirmed { get; set; }

        [Required]
        public int DentalTrainingId { get; set; }
        [ForeignKey("DentalTrainingId")]
        public DentalTraining DentalTraining { get; set; } = null!;

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    }
} 