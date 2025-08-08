using System.Threading.Tasks;
using Notification.Shared;

namespace Notification.Producer.Services
{
    public interface INotificationService
    {
        Task<bool> SendNotificationAsync(NotificationMessage notification);
        Task<bool> SendNotificationsAsync(NotificationMessage[] notifications);
    }
}
