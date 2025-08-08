using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Notification.Shared;
using Microsoft.Extensions.Configuration;

namespace Notification.Consumer.Handlers
{
    public class SmsNotificationHandler : INotificationHandler
    {
        private readonly string _authKey;
        private readonly string _senderId;
        private readonly string _route;
        private readonly string _country;

        public SmsNotificationHandler(IConfiguration config)
        {
            _authKey = config["MSG91:AuthKey"] ?? throw new ArgumentNullException("MSG91:AuthKey");
            _senderId = config["MSG91:SenderId"] ?? throw new ArgumentNullException("MSG91:SenderId");
            _route = config["MSG91:Route"] ?? "4";
            _country = config["MSG91:Country"] ?? "91";
        }

        public async Task HandleAsync(NotificationMessage notification)
        {
            if (string.IsNullOrWhiteSpace(notification.PhoneNumber)) return;
            using var client = new HttpClient();
            var payload = new
            {
                sender = _senderId,
                route = _route,
                country = _country,
                sms = new[]
                {
                    new {
                        message = notification.Message ?? string.Empty,
                        to = new[] { notification.PhoneNumber }
                    }
                }
            };
            var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.msg91.com/api/v2/sendsms")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("authkey", _authKey);
            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"MSG91 SMS sent to {notification.PhoneNumber}. Response: {result}");
        }
    }
}
