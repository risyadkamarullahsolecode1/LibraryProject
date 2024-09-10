using LibraryProject.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProject.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            if (string.IsNullOrEmpty(smtpSettings["Port"]))
            {
                throw new Exception("SMTP Port is not configured properly.");
            }

            if (!int.TryParse(smtpSettings["Port"], out int port))
            {
                throw new Exception("SMTP Port is not a valid number.");
            }

            using (var client = new SmtpClient(smtpSettings["Host"], port))
            {
                client.EnableSsl = bool.Parse(smtpSettings["EnableSsl"]);
                client.Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpSettings["Username"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
