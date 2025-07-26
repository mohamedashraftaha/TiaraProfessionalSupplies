namespace TiaraPro.Server.Authentication
{
    public interface IJWTToken
    {
        string GenerateToken(string email, string role);

        bool ValidateToken(string token);

        bool ValidateTokenExpiry(string token); 
    }
}
