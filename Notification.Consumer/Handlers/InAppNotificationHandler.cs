using System;
using System.Threading.Tasks;
using Notification.Shared;

namespace Notification.Consumer.Handlers
{
    public class InAppNotificationHandler : INotificationHandler
    {
        public Task HandleAsync(NotificationMessage notification)
        {
            // Simulate in-app notification (e.g., push to SignalR, DB, etc.)
            Console.WriteLine($"[IN-APP] {notification.Title}: {notification.Message} (User: {notification.UserId}, Order: {notification.OrderId})");
            return Task.CompletedTask;
        }
    }
}
