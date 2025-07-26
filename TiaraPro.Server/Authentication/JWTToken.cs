using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace TiaraPro.Server.Authentication;

public class JWTToken : IJWTToken
{

        private readonly IConfiguration _configuration;
        public JWTToken(IConfiguration configuration)
        {
            _configuration = configuration.GetSection("Jwt");
        }


    public bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["SecretKey"]);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Audience"],
                ClockSkew = TimeSpan.Zero // Optional: Set clock skew to zero for immediate expiration
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (emailClaim == null)
            {
                throw new SecurityTokenException("Email claim not found in token.");
            }
            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            if (roleClaim == null)
            {
                throw new SecurityTokenException("Role claim not found in token.");
            }

            if (roleClaim.Value != "Admin" && roleClaim.Value != "admin")
            {
                return false; // Only allow Admin or admin roles
            }
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    public bool ValidateTokenExpiry(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.ValidTo > DateTime.UtcNow;
        }
        catch (Exception)
        {
            return false; 
        }
    }
    public string GenerateToken(string email, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Role, role) 
        };

            var token = new JwtSecurityToken(
                issuer: _configuration["Issuer"],
                audience: _configuration["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
}
