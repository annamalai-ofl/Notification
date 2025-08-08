using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Notification.Shared;

namespace Notification.Producer.Services
{
    public interface INotificationHubService
    {
        Task SendInAppNotificationAsync(NotificationMessage notification);
    }

    public class NotificationHubService : INotificationHubService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationHubService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendInAppNotificationAsync(NotificationMessage notification)
        {
            if (!string.IsNullOrEmpty(notification.UserId))
            {
                await _hubContext.Clients.User(notification.UserId).SendAsync("ReceiveNotification", notification);
            }
            else
            {
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
            }
        }
    }
}
