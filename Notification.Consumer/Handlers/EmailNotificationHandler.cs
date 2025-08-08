using System;
using System.Threading.Tasks;
using Notification.Shared;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;

namespace Notification.Consumer.Handlers
{
    public class EmailNotificationHandler : INotificationHandler
    {
        private readonly string _apiKey;
        private readonly string _fromEmail;

        public EmailNotificationHandler(IConfiguration config)
        {
            _apiKey = config["SendGrid:ApiKey"] ?? throw new ArgumentNullException("SendGrid:ApiKey");
            _fromEmail = config["SendGrid:FromEmail"] ?? throw new ArgumentNullException("SendGrid:FromEmail");
        }

        public async Task HandleAsync(NotificationMessage notification)
        {
            if (string.IsNullOrWhiteSpace(notification.Email)) return;
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_fromEmail, "Notification Service");
            var to = new EmailAddress(notification.Email);
            var msg = MailHelper.CreateSingleEmail(
                from,
                to,
                notification.Title ?? "Notification",
                notification.Message ?? string.Empty,
                null
            );
            var response = await client.SendEmailAsync(msg);
            Console.WriteLine($"SendGrid email sent to {notification.Email} with status: {response.StatusCode}");
        }
    }
}
