using Confluent.Kafka;
using System;
using System.Text.Json;
using Notification.Shared;
using System.Threading;

namespace Notification.Producer
{
    public class KafkaNotificationProducer
    {
        public static void Produce(string broker, string topic, string connectionString, NotificationMessage notification)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = broker,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = "$ConnectionString",
                SaslPassword = connectionString,
                BrokerVersionFallback = "1.0.0"
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();
            var json = JsonSerializer.Serialize(notification);
            var dr = producer.ProduceAsync(topic, new Message<Null, string> { Value = json }).GetAwaiter().GetResult();
            Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
        }
    }
}
