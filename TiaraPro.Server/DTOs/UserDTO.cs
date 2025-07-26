namespace TiaraPro.Server.DTOs;

public class UserDTO
{
    public string FirstName { get; set; } = string.Empty;

    public string MiddleName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;


    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string? Role { get; set; } = "User";
    public string? Address { get; set; } = null;
    public string? City { get; set; } = null;
    public string? State { get; set; } = null;
    public string? PostalCode { get; set; } = null;

    public string? Phone { get; set; } = null;
    public string? Country { get; set; }
    public DateTimeOffset CreatedAt { get; set; } 

}
