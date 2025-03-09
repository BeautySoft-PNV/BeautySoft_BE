using MimeKit;
using MailKit.Net.Smtp;
namespace BeautySoftBE.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;
    
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        var smtpServer = _configuration["EmailSettings:SmtpServer"];
        var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
        var smtpUser = _configuration["EmailSettings:SmtpUser"];
        var smtpPass = _configuration["EmailSettings:SmtpPass"];

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Admin", smtpUser));
        email.To.Add(new MailboxAddress("", toEmail));
        email.Subject = subject;
        email.Body = new TextPart("html") { Text = message };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(smtpServer, smtpPort, false);
        await smtp.AuthenticateAsync(smtpUser, smtpPass);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
