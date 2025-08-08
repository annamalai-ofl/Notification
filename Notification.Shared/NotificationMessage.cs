using System;

namespace Notification.Shared
{

    public enum NotificationType
    {
        OrderPlaced,
        PaymentSuccessful,
        PaymentFailed,
        OrderCanceled,
        OrderAccepted,
        OrderRejected,
        // Add more scenarios as needed
        
    }

    public enum NotificationChannel
    {
        InApp,
        Email,
        SMS,
        WhatsApp
    }

    public class NotificationMessage
    {
        public NotificationType Type { get; set; }
        public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;
        public string? Title { get; set; }
        public string? Message { get; set; }
        public string? UserId { get; set; }
        public string? OrderId { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
