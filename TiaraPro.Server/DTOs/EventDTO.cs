namespace TiaraPro.Server.DTOs
{
    public class EventDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? Speakers { get; set; }
        public DateTime Date { get; set; }
        public string? Location { get; set; }
        public string? ImageUrl { get; set; }
        public int? Capacity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 