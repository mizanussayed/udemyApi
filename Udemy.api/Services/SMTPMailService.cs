using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Udemy.api.Models.Dto;
using Udemy.api.Settings;

namespace Udemy.api.Services;

public class SMTPMailService : IMailService
{
    private readonly MailSettings _mailSettings;
    private readonly ILogger<SMTPMailService> _logger ;

    public SMTPMailService(IOptions<MailSettings> mailSettings, ILogger<SMTPMailService> logger)
    {
        _mailSettings = mailSettings.Value;
        _logger = logger;
    }

    public async Task SendAsync(MailRequest request)
    {
        try
        {
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(request.From ?? _mailSettings.From)
            };
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;
            var builder = new BodyBuilder
            {
                HtmlBody = request.Body
            };
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.UserName, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}
