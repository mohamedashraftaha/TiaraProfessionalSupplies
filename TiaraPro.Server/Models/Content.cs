using System.ComponentModel.DataAnnotations;

namespace TiaraPro.Server.Models
{
    public class Content
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Page { get; set; } = string.Empty;
        [Required]
        public string Key { get; set; } = string.Empty;
        [Required]
        public string Value { get; set; } = string.Empty;
    }
} 