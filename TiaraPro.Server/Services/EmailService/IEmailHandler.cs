namespace TiaraPro.Server.Services.EmailService;

public interface IEmailHandler
{
    Task SendEmailAsync(string toEmail, string subject, string body, string? cc = null);

} 