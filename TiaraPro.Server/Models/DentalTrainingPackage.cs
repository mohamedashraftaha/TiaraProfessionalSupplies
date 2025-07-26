using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiaraPro.Server.Models
{
    public class DentalTrainingPackage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DentalTrainingId { get; set; }
        [ForeignKey("DentalTrainingId")]
        public DentalTraining DentalTraining { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
} 