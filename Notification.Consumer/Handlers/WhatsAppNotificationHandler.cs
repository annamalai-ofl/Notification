
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Notification.Shared;
using Microsoft.Extensions.Configuration;

namespace Notification.Consumer.Handlers
{
    public class WhatsAppNotificationHandler : INotificationHandler
    {
        private readonly string _authKey;
        private readonly string _senderId;

        public WhatsAppNotificationHandler(IConfiguration config)
        {
            _authKey = config["MSG91:AuthKey"] ?? throw new ArgumentNullException("MSG91:AuthKey");
            _senderId = config["MSG91:SenderId"] ?? throw new ArgumentNullException("MSG91:SenderId");
        }

        public async Task HandleAsync(NotificationMessage notification)
        {
            if (string.IsNullOrWhiteSpace(notification.PhoneNumber)) return;
            using var client = new HttpClient();
            var payload = new
            {
                to = new[] { notification.PhoneNumber },
                sender = _senderId,
                type = "text",
                message = notification.Message ?? string.Empty
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.msg91.com/api/v1/whatsapp/send")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("authkey", _authKey);
            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"MSG91 WhatsApp message sent to {notification.PhoneNumber}. Response: {result}");
        }
    }
}
