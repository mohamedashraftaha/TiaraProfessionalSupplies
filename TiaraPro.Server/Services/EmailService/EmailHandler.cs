using MailKit.Net.Smtp;
using MimeKit;
using System.Diagnostics;
using SendGrid;
using SendGrid.Helpers.Mail;
namespace TiaraPro.Server.Services.EmailService;

public class EmailHandler : IEmailHandler
{
    private readonly IConfiguration _config;

    public EmailHandler(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, string? cc = null)
    {
        var sw = Stopwatch.StartNew();

        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_config["EmailSettings:From"]));
        email.To.Add(MailboxAddress.Parse(toEmail));
        if (!string.IsNullOrEmpty(cc))
        {
            email.Cc.Add(MailboxAddress.Parse(cc));
        }
        email.Subject = subject;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

        using var smtp = new SmtpClient();
        try
        {
            Console.WriteLine("Starting Connect...");
            sw.Restart();
            await smtp.ConnectAsync(
                _config["EmailSettings:SmtpServer"],
                int.Parse(_config["EmailSettings:Port"]),
                MailKit.Security.SecureSocketOptions.StartTls
            );
            Console.WriteLine($"Connected in {sw.ElapsedMilliseconds} ms");

            Console.WriteLine("Starting Authenticate...");
            sw.Restart();
            await smtp.AuthenticateAsync(
                _config["EmailSettings:Username"],
                _config["EmailSettings:Password"]
            );
            Console.WriteLine($"Authenticated in {sw.ElapsedMilliseconds} ms");

            Console.WriteLine("Starting Send...");
            sw.Restart();
            await smtp.SendAsync(email);
            Console.WriteLine($"Sent in {sw.ElapsedMilliseconds} ms");

            await smtp.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to send email", ex);
        }
    }

    // Sendgrid
    //public async Task SendEmailAsync(string toEmail, string subject, string body)
    //{
    //    var sw = Stopwatch.StartNew();
    //    try
    //    {
    //        var apiKey = _config["EmailSettings:Password"];
    //        var client = new SendGridClient(apiKey);
    //        var from = new EmailAddress(_config["EmailSettings:From"], "Tiara Professional Supplies");
    //        var emailAddressTo = new List<EmailAddress> {
    //            {new EmailAddress(toEmail)   },
    //            {new EmailAddress("sales@tiarapro.com") }

    //        };
    //        //var to = new EmailAddress(toEmail);
    //        var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, emailAddressTo, subject, null, body);

    //        Console.WriteLine("Starting SendGrid Send...");
    //        sw.Restart();
    //        var response = await client.SendEmailAsync(msg);
    //        Console.WriteLine($"SendGrid sent in {sw.ElapsedMilliseconds} ms");

    //        if (!response.IsSuccessStatusCode)
    //        {
    //            var errorBody = await response.Body.ReadAsStringAsync();
    //            throw new InvalidOperationException($"SendGrid failed: {response.StatusCode} - {errorBody}");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new InvalidOperationException("Failed to send email via SendGrid", ex);
    //    }
    //}
} 