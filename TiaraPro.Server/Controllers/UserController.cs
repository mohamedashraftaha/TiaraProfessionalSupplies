using Microsoft.AspNetCore.Mvc;
using TiaraPro.Server.Authentication;
using TiaraPro.Server.DTOs;
using TiaraPro.Server.Models;
using TiaraPro.Server.Services.UsersService;

namespace TiaraPro.Server.Controllers;

[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly IJWTToken _tokenService;
    public UserController(ILogger<UserController> logger, IUserService userService, IJWTToken tokenService)
    {
        _logger = logger;
        _userService = userService;
        _tokenService = tokenService;
    }


    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(int userId)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user by ID.");
            return StatusCode(500, new { message = "Internal server error." });
        }
    }

    [HttpPost("ValidateTokenExpiry")]
    public async Task<IActionResult> ValidateTokenExpiry([FromBody] string token)
    {
        try
        {
            var isValid = _tokenService.ValidateTokenExpiry(token);
            if (isValid)
            {
                return Ok(new { message = "Token is valid." });
            }
            return Unauthorized(new { message = "Token has expired." });
        }
        catch (Exception ex)
        {
            _logger.LogError("something went wrong {ExceptionMessage}", ex.Message);
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }

    [HttpPost("validateToken")]
    public async Task<IActionResult> ValidateToken([FromBody] string token)
    {
        try
        {
            var isValid = _tokenService.ValidateToken(token);
            if (isValid)
            {
                return Ok(new { message = "Token is valid." });
            }
            return Unauthorized(new { message = "Invalid token." });
        }
        catch(Exception ex)
        {
            _logger.LogError("something went wrong {ExceptionMessage}", ex.Message);
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }


    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _userService.GetUsersAsync();
        if (result == null || result.Count == 0)
        {
            return NotFound(new { message = "No users found." });
        }
        return Ok(result);
    }

    [HttpPost("updateUser")]
    public async Task<IActionResult> UpdateUser([FromBody] UserDTO user)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _userService.UpdateUserAsync(user);
            if (result)
            {
                return Ok(new { message = "User updated successfully." });
            }
            return NotFound(new { message = "User not found." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user.");
            return StatusCode(500, new { message = "Internal server error." });
        }
    }


        [HttpPost("signup")]
    public async Task<IActionResult> SignUpUser([FromBody] UserDTO user)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _userService.SignUpUserAsync(user);
            if (result)
            {
                return CreatedAtAction(nameof(SignUpUser), new { email = user.Email });
            }
            return StatusCode(409, new { message = "User already exists." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signing up user.");
            return StatusCode(500, new { message = "Internal server error." });
        }
    }

    [HttpPost("signin-admin")]
    public async Task<IActionResult> SignInUserAdmin([FromBody] UserDTO user)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.SignInAdminAsync(user);
            if (result.UserId != 0)
            {
                var token = _tokenService.GenerateToken(user.Email, result.Role!);
                return Ok(
                    new
                    {
                        message = "Sign in successful.",
                        firstName = result.FirstName,
                        lastName = result.LastName,
                        email = user.Email,
                        role = result.Role,
                        userId = result.UserId.ToString(),
                        address = result.Address,
                        city = result.City,
                        state = result.State,
                        country = result.Country,
                        postalCode = result.PostalCode,
                        createdAt = result.CreatedAt,
                        phone = result.Phone,
                        token = token
                    }
                );

            }

            return Unauthorized(new { message = "Invalid email or password." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signing in user.");
            return StatusCode(500, new { message = "Internal server error." });
        }
    }


    [HttpPost("signin")]
    public async Task<IActionResult> SignInUser([FromBody] UserDTO user)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.SignInUserAsync(user);
            if (result.UserId != 0)
            {
                var token = _tokenService.GenerateToken(user.Email, result.Role!);
                return Ok(
                    new
                    {
                        message = "Sign in successful.",
                        firstName = result.FirstName,
                        middleName = result.MiddleName,
                        lastName = result.LastName,
                        email = user.Email,
                        role= user.Role,
                        userId = result.UserId.ToString(),
                        address = result.Address,
                        city = result.City,
                        state = result.State,
                        country = result.Country,
                        postalCode = result.PostalCode,
                        createdAt = result.CreatedAt,
                        phone = result.Phone,
                        tiaraAiActive = result.TiaraAiActive,
                        token = token
                    }
                );

            }

            return Unauthorized(new { message = "Invalid email or password." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signing in user.");
            return StatusCode(500, new { message = "Internal server error." });
        }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userService.ForgotPasswordAsync(dto.Email);
        if (result)
            return Ok(new { message = "If the email exists, a reset code has been sent." });
        return StatusCode(500, new { message = "Failed to process password reset request." });
    }

    [HttpPost("verify-reset-code")]
    public async Task<IActionResult> VerifyResetCode([FromBody] ResetPasswordDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userService.VerifyResetCodeAsync(dto.Email, dto.VerificationCode);
        if (result)
            return Ok(new { message = "Code is valid." });
        return BadRequest(new { message = "Invalid or expired code." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userService.ResetPasswordAsync(dto.Email, dto.VerificationCode, dto.NewPassword);
        if (result)
            return Ok(new { message = "Password has been reset successfully." });
        return BadRequest(new { message = "Invalid or expired code, or failed to reset password." });
    }

}
