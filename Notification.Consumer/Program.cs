using Notification.Consumer;
using Notification.Consumer.Data;

using SendGrid;
using SendGrid.Helpers.Mail;
using Notification.Consumer.Handlers;

using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Notification.Shared;

class Program
{
    private const string consumerGroup = "<CONSUMER_GROUP>"; // Set your consumer group

    static async Task Main(string[] args)
    {
        // Build configuration
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Uncomment one of the following blocks depending on protocol

        // --- Native Event Hubs SDK (default) ---
        var eventHubConnectionString = config["EventHub:ConnectionString"];
        var eventHubName = config["EventHub:Name"];
        var storageConnectionString = config["Storage:ConnectionString"];
        var storageContainerName = config["Storage:ContainerName"];

        // Instantiate handlers
        var emailHandler = new EmailNotificationHandler(config);
        var smsHandler = new SmsNotificationHandler(config);
        var whatsappHandler = new WhatsAppNotificationHandler(config);
        // Use EF Core handler for in-app notifications
        var dbContext = DbContextFactory.CreateDbContext();
        var inAppHandler = new EfInAppNotificationHandler(dbContext);

        var blobContainerClient = new BlobContainerClient(storageConnectionString, storageContainerName);
        var processor = new EventProcessorClient(
            blobContainerClient,
            consumerGroup,
            eventHubConnectionString,
            eventHubName);

        processor.ProcessEventAsync += async (ProcessEventArgs eventArgs) =>
        {
            var json = Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray());
            var notification = JsonSerializer.Deserialize<NotificationMessage>(json);
            if (notification != null)
            {
                Console.WriteLine($"[{notification.Timestamp:u}] {notification.Type}: {notification.Title} - {notification.Message} (User: {notification.UserId}, Order: {notification.OrderId})");

                switch (notification.Channel)
                {
                    case NotificationChannel.Email:
                        await emailHandler.HandleAsync(notification);
                        break;
                    case NotificationChannel.SMS:
                        await smsHandler.HandleAsync(notification);
                        break;
                    case NotificationChannel.WhatsApp:
                        await whatsappHandler.HandleAsync(notification);
                        break;
                    case NotificationChannel.InApp:
                    default:
                        await inAppHandler.HandleAsync(notification);
                        break;
                }
                // Update checkpoint after successful processing
                await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
            }
            else
            {
                Console.WriteLine("Received a message that could not be deserialized to NotificationMessage.");
            }
        };

        processor.ProcessErrorAsync += async (ProcessErrorEventArgs eventArgs) =>
        {
            Console.WriteLine($"Error in partition '{eventArgs.PartitionId}': {eventArgs.Exception.Message}");
            await Task.CompletedTask;
        };

        Console.WriteLine("Listening for notifications (with checkpointing)...");
        await processor.StartProcessingAsync();

        Console.WriteLine("Press [Enter] to stop...");
        Console.ReadLine();
        await processor.StopProcessingAsync();

        // --- Kafka protocol support ---
        // var broker = "<NAMESPACE>.servicebus.windows.net:9093";
        // var topic = config["EventHub:Name"];
        // var connectionString = config["EventHub:ConnectionString"];
        // KafkaNotificationConsumer.Consume(broker, topic, connectionString, consumerGroup);
    }
}
