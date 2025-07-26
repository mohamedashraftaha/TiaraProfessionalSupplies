namespace TiaraPro.Server.DTOs
{
    using System.Collections.Generic;

    public class DentalTrainingDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? Instructors { get; set; }
        public DateTime Date { get; set; }
        public string? Location { get; set; }
        public string? ImageUrl { get; set; }
        public int? Capacity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<DentalTrainingPackageDTO>? Packages { get; set; }
    }
} 