namespace TiaraPro.Server.Models;

public class SignInUserResponseModel
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;

    public string MiddleName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string? Email { get; set; } = string.Empty;
    public string? Role { get; set; } = "User";
    public string? Address { get; set; } = null;
    public string? City { get; set; } = null;
    public string? State { get; set; } = null;
    public string? PostalCode { get; set; } = null;
    public string? Country { get; set; } = null;
    public DateTimeOffset? CreatedAt { get; set; }

    public string? Phone { get; set; } = null;

    public bool TiaraAiActive { get; set; }


}
