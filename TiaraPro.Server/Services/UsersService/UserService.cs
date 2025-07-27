using TiaraPro.Server.DTOs;
using TiaraPro.Server.Models;
using TiaraPro.Server.PersistenceLayer.UnitOfWork;
using TiaraPro.Server.Utils;
using TiaraPro.Server.Services.EmailService;

namespace TiaraPro.Server.Services.UsersService;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailHandler _emailHandler;

    public UserService(ILogger<UserService> logger, IUnitOfWork unitOfWork, IEmailHandler emailHandler)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _emailHandler = emailHandler;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by ID: {UserId}", userId);
            return null;
        }
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by email: {Email}", email);
            return null;
        }
    }


    public async Task<bool> SignUpUserAsync(UserDTO user)
    {
        try
        {
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(user.Email);
            if (existingUser != null)
            {
                return false;
            }
            var newUser = new User
            {
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email,
                PasswordHash = PasswordHelper.HashPassword(user.PasswordHash),
                Address = user.Address,
                City = user.City,
                State = user.State,
                Phone = user.Phone,
                Country = user.Country
            };
            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Users.AddAsync(newUser);
            int rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected == 0)
            {
                await _unitOfWork.RollbackAsync();
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signing up user.");
            await _unitOfWork.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> UpdateUserAsync(UserDTO user)
    {
        try
        {
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(user.Email);
            if (existingUser == null)
            {
                return false;
            }
            existingUser.FirstName = user.FirstName;
            existingUser.MiddleName = user.MiddleName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.Address = user.Address;
            existingUser.City = user.City;
            existingUser.State = user.State;
            existingUser.Phone = user.Phone;
            existingUser.Country = user.Country;
            existingUser.PostalCode = user.PostalCode;
            existingUser.Role = user.Role;

            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Users.Update(existingUser);
            int rowsAffected = await _unitOfWork.CompleteAsync();
            if (rowsAffected == 0)
            {
                await _unitOfWork.RollbackAsync();
                return false;
            }
            return true;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user.");
            return false;
        }
    }
    public async Task<List<User>> GetUsersAsync()
    {
        try
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users.");
            throw;
        }
    }

    public async Task<SignInUserResponseModel> SignInAdminAsync(UserDTO user)
    {
        try
        {
            var res = 0;
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(user.Email);

            if (existingUser == null || !PasswordHelper.VerifyPassword(user.PasswordHash, existingUser.PasswordHash) || existingUser.Role != "Admin")
            {
                return new SignInUserResponseModel
                {
                    UserId = res,
                    FirstName = "",
                    LastName = "",
                    Email = "",
                };

            }


            //#region Generate Verification Code
            //if (await CreateUserVerificationCode(existingUser))
            //{
            //    // Send verification code to user via email
            //    string emailBody = $@"
            //<html>
            //    <body>
            //        <h2>Your Verification Code</h2>
            //        <p>Dear {existingUser.Email},</p>
            //        <p>Thank you for signing in. To complete the process, please use the following verification code:</p>
            //        <h3>{existingUser.VerificationCode}</h3>
            //        <p>This code is valid for 5 minutes. If you did not request this code, please ignore this email.</p>
            //        <p>Best regards,<br />EMRA</p>
            //        {EmailContentGenerator.GetEmailSignature()}
            //    </body>
            //</html>";
            //    await _emailHandler.SendEmailAsync(existingUser.Email, "Verification Code", emailBody);
            //}
            //else
            //{
            //    return false;
            //}
            //#endregion

            return new SignInUserResponseModel
            {
                UserId = existingUser.Id,
                FirstName = existingUser.FirstName!,
                MiddleName = existingUser.MiddleName!,
                LastName = existingUser.LastName!,
                Email = existingUser.Email!,
                Role = existingUser.Role!,
                Address = existingUser.Address,
                City = existingUser.City,
                State = existingUser.State,
                PostalCode = existingUser.PostalCode,
                Country = existingUser.Country,
                CreatedAt = existingUser.CreatedAt,
                Phone = existingUser.Phone,

            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signing in user.");
            return new SignInUserResponseModel
            {
                UserId = 0,
                FirstName = "",
                LastName = "",
            };
        }
    }

    public async Task<SignInUserResponseModel> SignInUserAsync(UserDTO user)
    {
        try
        {
            var res = 0;
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(user.Email);

            if (existingUser == null || !PasswordHelper.VerifyPassword(user.PasswordHash, existingUser.PasswordHash))
            {
                return new SignInUserResponseModel
                {
                    UserId = res,
                    FirstName = "",
                    LastName = "",
                    Email = "",
                };

            }


            //#region Generate Verification Code
            //if (await CreateUserVerificationCode(existingUser))
            //{
            //    // Send verification code to user via email
            //    string emailBody = $@"
            //<html>
            //    <body>
            //        <h2>Your Verification Code</h2>
            //        <p>Dear {existingUser.Email},</p>
            //        <p>Thank you for signing in. To complete the process, please use the following verification code:</p>
            //        <h3>{existingUser.VerificationCode}</h3>
            //        <p>This code is valid for 5 minutes. If you did not request this code, please ignore this email.</p>
            //        <p>Best regards,<br />EMRA</p>
            //        {EmailContentGenerator.GetEmailSignature()}
            //    </body>
            //</html>";
            //    await _emailHandler.SendEmailAsync(existingUser.Email, "Verification Code", emailBody);
            //}
            //else
            //{
            //    return false;
            //}
            //#endregion

            return new SignInUserResponseModel
            {
                UserId = existingUser.Id,
                FirstName = existingUser.FirstName!,
                MiddleName = existingUser.MiddleName!,
                LastName = existingUser.LastName!,
                Email = existingUser.Email!,
                Role = existingUser.Role!,
                Address = existingUser.Address,
                City = existingUser.City,
                State = existingUser.State,
                PostalCode = existingUser.PostalCode,
                Country = existingUser.Country,
                CreatedAt = existingUser.CreatedAt,
                Phone = existingUser.Phone,
                TiaraAiActive = existingUser.TiaraAiActive.GetValueOrDefault()

            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signing in user.");
            return new SignInUserResponseModel
            {
                UserId = 0,
                FirstName = "",
                LastName = "",
            };
        }
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
            {
                // Don't reveal if email exists or not for security
                _logger.LogWarning("Password reset requested for non-existent email: {Email}", email);
                return true; // Return true to not reveal if email exists
            }

            // Generate 6-digit verification code
            var verificationCode = GenerateVerificationCode();

            // Set expiration time (15 minutes from now)
            var expirationTime = DateTime.UtcNow.AddMinutes(15);

            // Update user with verification code and expiration
            user.VerificationCode = verificationCode;
            user.CodeExpiration = expirationTime;

            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();

            // Send email with verification code
            var emailSubject = "TiaraPro - Password Reset Code";
            var emailBody = GeneratePasswordResetEmail(user.FirstName ?? "User", verificationCode);

            await _emailHandler.SendEmailAsync(email, emailSubject, emailBody);

            _logger.LogInformation("Password reset code sent to email: {Email}", email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing forgot password request for email: {Email}", email);
            await _unitOfWork.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> VerifyResetCodeAsync(string email, string verificationCode)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            // Check if code matches and hasn't expired
            if (user.VerificationCode != verificationCode ||
                user.CodeExpiration == null ||
                user.CodeExpiration < DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying reset code for email: {Email}", email);
            return false;
        }
    }

    public async Task<bool> ResetPasswordAsync(string email, string verificationCode, string newPassword)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            // Verify the code first
            if (!await VerifyResetCodeAsync(email, verificationCode))
            {
                return false;
            }

            // Update password and clear verification code
            user.PasswordHash = PasswordHelper.HashPassword(newPassword);
            user.VerificationCode = null;
            user.CodeExpiration = null;

            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();
                
            // Send confirmation email
            var emailSubject = "TiaraPro - Password Reset Successful";
            var emailBody = GeneratePasswordResetSuccessEmail(user.FirstName ?? "User");

            await _emailHandler.SendEmailAsync(email, emailSubject, emailBody);

            _logger.LogInformation("Password reset successful for email: {Email}", email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for email: {Email}", email);
            await _unitOfWork.RollbackAsync();
            return false;
        }
    }

    private string GenerateVerificationCode()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    private string GeneratePasswordResetEmail(string firstName, string verificationCode)
    {
        return $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                <div style='text-align: center; margin-bottom: 30px;'>
                    <img src='https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/Tiara-logo-original.png' alt='TiaraPro Logo' style='width: 120px; height: auto;' />
                </div>
                
                <div style='background: linear-gradient(135deg, #1e3a8a, #4052B5); color: white; padding: 30px; border-radius: 10px; text-align: center; margin-bottom: 30px;'>
                    <h1 style='margin: 0; font-size: 24px;'>Password Reset Request</h1>
                </div>
                
                <div style='padding: 20px; background: #f8fafc; border-radius: 10px; margin-bottom: 30px;'>
                    <p style='font-size: 16px; color: #334155; margin-bottom: 20px;'>Hello {firstName},</p>
                    
                    <p style='font-size: 16px; color: #334155; margin-bottom: 20px;'>
                        We received a request to reset your password for your TiaraPro account. 
                        Use the verification code below to reset your password:
                    </p>
                    
                    <div style='text-align: center; margin: 30px 0;'>
                        <div style='display: inline-block; background: #1e3a8a; color: white; padding: 15px 30px; border-radius: 8px; font-size: 24px; font-weight: bold; letter-spacing: 3px;'>
                            {verificationCode}
                        </div>
                    </div>
                    
                    <p style='font-size: 14px; color: #64748b; text-align: center; margin-bottom: 20px;'>
                        This code will expire in 15 minutes for security reasons.
                    </p>
                    
                    <p style='font-size: 16px; color: #334155; margin-bottom: 10px;'>
                        If you didn't request this password reset, please ignore this email or contact our support team.
                    </p>
                </div>
                
                <div style='text-align: center; padding: 20px; color: #64748b; font-size: 14px;'>
                    <p>Best regards,<br/>The TiaraPro Team</p>
                    <p style='margin-top: 20px;'>
                        <a href='mailto:support@tiarapro.com' style='color: #1e3a8a;'>support@tiarapro.com</a>
                    </p>
                </div>
            </div>";
    }

    private string GeneratePasswordResetSuccessEmail(string firstName)
    {
        return $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                <div style='text-align: center; margin-bottom: 30px;'>
                    <img src='https://tiaraprofessionalsupplies.s3.eu-central-1.amazonaws.com/Tiara-AI-logo-original.svg' alt='TiaraPro Logo' style='width: 120px; height: auto;' />
                </div>
                
                <div style='background: linear-gradient(135deg, #059669, #10b981); color: white; padding: 30px; border-radius: 10px; text-align: center; margin-bottom: 30px;'>
                    <h1 style='margin: 0; font-size: 24px;'>Password Reset Successful</h1>
                </div>
                
                <div style='padding: 20px; background: #f0fdf4; border-radius: 10px; margin-bottom: 30px;'>
                    <p style='font-size: 16px; color: #334155; margin-bottom: 20px;'>Hello {firstName},</p>
                    
                    <p style='font-size: 16px; color: #334155; margin-bottom: 20px;'>
                        Your password has been successfully reset for your TiaraPro account.
                    </p>
                    
                    <p style='font-size: 16px; color: #334155; margin-bottom: 20px;'>
                        You can now log in to your account using your new password.
                    </p>
                    
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='https://tiarapro.com/login' style='display: inline-block; background: #1e3a8a; color: white; padding: 12px 30px; border-radius: 8px; text-decoration: none; font-weight: bold;'>
                            Login to Your Account
                        </a>
                    </div>
                    
                    <p style='font-size: 14px; color: #64748b; margin-bottom: 10px;'>
                        If you didn't make this change or if you have any concerns about your account security, 
                        please contact our support team immediately.
                    </p>
                </div>
                
                <div style='text-align: center; padding: 20px; color: #64748b; font-size: 14px;'>
                    <p>Best regards,<br/>The TiaraPro Team</p>
                    <p style='margin-top: 20px;'>
                        <a href='mailto:support@tiarapro.com' style='color: #1e3a8a;'>support@tiarapro.com</a>
                    </p>
                </div>
            </div>";
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _unitOfWork.Users.GetAllAsync();
    }
}
