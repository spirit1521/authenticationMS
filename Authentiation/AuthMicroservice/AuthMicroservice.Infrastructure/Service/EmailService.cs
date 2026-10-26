using AuthMicroservice.Application.DTO;
using AuthMicroservice.Application.Interface;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace AuthMicroservice.Infrastructure.Service
{
    public class EmailService : IEmailService
    {
        EmailSettingsDto settings;
        public EmailService(IOptions<EmailSettingsDto> options)
        {
            settings = options.Value;
        }

        public async Task SendResetToken(string email, string token)
        {
            using var client = new SmtpClient(settings.Host, settings.Port);

            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(settings.Email, settings.Password);
            using var mail = new MailMessage(settings.Email, email);
            mail.Subject = "Reset your password";
            mail.Body = $"Your reset token is: {token}\n\nIt expires in 15 minutes.";
            await client.SendMailAsync(mail);
        }
    }
}
