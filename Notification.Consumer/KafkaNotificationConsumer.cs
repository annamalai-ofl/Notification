using Confluent.Kafka;
using System;
using System.Text.Json;
using System.Threading;
using Notification.Shared;

namespace Notification.Consumer
{
    public class KafkaNotificationConsumer
    {
        public static void Consume(string broker, string topic, string connectionString, string consumerGroup)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = broker,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "$ConnectionString",
                SaslPassword = connectionString,
                GroupId = consumerGroup,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                BrokerVersionFallback = "1.0.0",
                EnablePartitionEof = true
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(topic);
            Console.WriteLine($"Consuming from topic '{topic}'...");
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };
            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    var cr = consumer.Consume(cts.Token);
                    if (cr.IsPartitionEOF) continue;
                    var notification = JsonSerializer.Deserialize<NotificationMessage>(cr.Message.Value);
                    if (notification != null)
                    {
                        Console.WriteLine($"[{notification.Timestamp:u}] {notification.Type}: {notification.Title} - {notification.Message} (User: {notification.UserId}, Order: {notification.OrderId})");
                        // TODO: Call handlers as in your main consumer
                    }
                }
            }
            catch (OperationCanceledException) { }
            finally { consumer.Close(); }
        }
    }
}
