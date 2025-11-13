using Eventify.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _config;

        public EmailService(ILogger<EmailService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            var apiKey = _config["Notification:SendGridApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_config["Notification:FromEmail"], "Eventify");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            var response = await client.SendEmailAsync(msg);

            _logger.LogInformation($"Email sent to {toEmail} with status {response.StatusCode}");
        }
    }
}
