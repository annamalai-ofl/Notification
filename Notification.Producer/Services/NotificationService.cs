using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Notification.Shared;

namespace Notification.Producer.Services
{
    public class NotificationService : INotificationService
    {
        private readonly EventHubProducerClient _producerClient;
        private readonly INotificationHubService _hubService;
        public NotificationService(EventHubProducerClient producerClient, INotificationHubService hubService)
        {
            _producerClient = producerClient;
            _hubService = hubService;
        }

        public async Task<bool> SendNotificationAsync(NotificationMessage notification)
        {
            // Send to Event Hub as before
            using EventDataBatch eventBatch = await _producerClient.CreateBatchAsync();
            var json = JsonSerializer.Serialize(notification);
            if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(json))))
            {
                return false;
            }
            await _producerClient.SendAsync(eventBatch);

            // If in-app, also send via SignalR
            if (notification.Channel == NotificationChannel.InApp)
            {
                await _hubService.SendInAppNotificationAsync(notification);
            }
            return true;
        }

        public async Task<bool> SendNotificationsAsync(NotificationMessage[] notifications)
        {
            using EventDataBatch eventBatch = await _producerClient.CreateBatchAsync();
            foreach (var notification in notifications)
            {
                var json = JsonSerializer.Serialize(notification);
                if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(json))))
                {
                    return false;
                }
            }
            await _producerClient.SendAsync(eventBatch);
            return true;
        }
    }
}
