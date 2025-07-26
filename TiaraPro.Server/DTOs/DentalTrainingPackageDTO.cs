namespace TiaraPro.Server.DTOs
{
    public class DentalTrainingPackageDTO
    {
        public int Id { get; set; }
        public int DentalTrainingId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
    }
} 