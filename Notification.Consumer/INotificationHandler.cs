using System.Threading.Tasks;
using Notification.Shared;

namespace Notification.Consumer
{
    public interface INotificationHandler
    {
        Task HandleAsync(NotificationMessage notification);
    }
}
