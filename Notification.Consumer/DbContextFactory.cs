using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Notification.Consumer.Data;

namespace Notification.Consumer
{
    public static class DbContextFactory
    {
        public static NotificationDbContext CreateDbContext()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<NotificationDbContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("NotificationDb"));
            return new NotificationDbContext(optionsBuilder.Options);
        }
    }
}
