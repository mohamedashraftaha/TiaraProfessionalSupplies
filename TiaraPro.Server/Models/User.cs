using System.ComponentModel.DataAnnotations;

namespace TiaraPro.Server.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;
        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; } = null!;
        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? MiddleName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }
        [StringLength(20)]
        public string? Phone { get; set; }
        [StringLength(200)]
        public string? Role { get; set; } = "User";
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public bool? TiaraAiActive { get; set; } = false;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public List<Transactions>? Transactions { get; set; } = new();
        public ICollection<Order>? Orders { get; set; }
        public string? VerificationCode { get; set; }
        public DateTime? CodeExpiration { get; set; }
        public bool IsVerified { get; set; } = false;
    }
}
