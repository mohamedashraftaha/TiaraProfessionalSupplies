using TiaraPro.Server.DTOs;
using TiaraPro.Server.Models;

namespace TiaraPro.Server.Services.UsersService;

public interface IUserService
{
    Task<bool> SignUpUserAsync(UserDTO user);

    Task<User?> GetUserByIdAsync(int userId);
    Task<SignInUserResponseModel> SignInAdminAsync(UserDTO user);

    Task<SignInUserResponseModel> SignInUserAsync(UserDTO user);
    Task<bool> ForgotPasswordAsync(string email);
    Task<bool> ResetPasswordAsync(string email, string verificationCode, string newPassword);
    Task<bool> VerifyResetCodeAsync(string email, string verificationCode);

    Task<List<User>> GetUsersAsync();

    Task<bool> UpdateUserAsync(UserDTO user);

    Task<User?> GetUserByEmailAsync(string email);

    Task<List<User>> GetAllUsersAsync();
}
