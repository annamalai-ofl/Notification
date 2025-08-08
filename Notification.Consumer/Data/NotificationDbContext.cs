using Microsoft.EntityFrameworkCore;

namespace Notification.Consumer.Data
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

        public DbSet<NotificationEntity> Notifications { get; set; }
    }
}
