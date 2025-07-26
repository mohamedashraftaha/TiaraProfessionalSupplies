using System.ComponentModel.DataAnnotations;

namespace TiaraPro.Server.DTOs;

public class ForgotPasswordDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
} 