using Contracts;
using Entities.ConfigurationModels;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using Service.Contracts;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    internal class EmailSenderService : IEmailSenderService
    {
        private readonly ILoggerManager _logger;
        private readonly IConfiguration _configuration;
        private readonly EmailConfiguration _emailConfiguration;

        public EmailSenderService(ILoggerManager logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _emailConfiguration = new EmailConfiguration();
            _configuration.Bind(_emailConfiguration.Section, _emailConfiguration);
        }
        public async Task SendSMTP(EmailSenderDto emailDto)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(emailDto.From));
            email.To.Add(MailboxAddress.Parse(emailDto.To));
            email.Subject = emailDto.Subject;
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = EmailBodyHTML(emailDto)
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailConfiguration.Host, 
                    _emailConfiguration.Port, 
                    MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailConfiguration.UserName, _emailConfiguration.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        private string EmailBodyHTML(EmailSenderDto emailDto)
        {
            string template = Path.
               Combine(Directory.GetCurrentDirectory(),
               "Email", "templates", $"{_emailConfiguration.Template}.html");
            string htmlContent = File.ReadAllText(template);
            htmlContent = htmlContent.Replace("__textbody__", emailDto.Body);
            
            return htmlContent;
        }
    }
}
