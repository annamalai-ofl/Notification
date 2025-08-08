using System.Threading.Tasks;
using Notification.Shared;
using Notification.Consumer.Data;

namespace Notification.Consumer.Handlers
{
    public class EfInAppNotificationHandler : INotificationHandler
    {
        private readonly NotificationDbContext _dbContext;
        public EfInAppNotificationHandler(NotificationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task HandleAsync(NotificationMessage notification)
        {
            var entity = new NotificationEntity
            {
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                OrderId = notification.OrderId,
                Timestamp = notification.Timestamp,
                Channel = notification.Channel.ToString(),
                Type = notification.Type.ToString()
            };
            _dbContext.Notifications.Add(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
