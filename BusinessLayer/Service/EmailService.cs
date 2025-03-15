using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using BusinessLayer.Interface;

namespace BusinessLayer.Service
{
    public class EmailService:IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendResetEmail(string email, string token)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Recipient email cannot be null or empty", nameof(email));
            }
            var smtpHost = _configuration["Smtp:Host"];
            var smtpPort = int.Parse(_configuration["Smtp:Port"]);
            var smtpUser = _configuration["Smtp:Username"];
            var smtpPass = _configuration["Smtp:Password"];

            var smtpClient = new SmtpClient(smtpHost)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true,
            };

            string resetLink = $"https://AddressBook.com/reset-password?token={token}";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUser),
                Subject = "Password Reset Request",
                Body = $"Click the link to reset your password: <a href='{resetLink}'>Reset Password</a>",
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
