using System;
using System.ComponentModel.DataAnnotations;

namespace Notification.Consumer.Data
{
    public class NotificationEntity
    {
        [Key]
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public string? OrderId { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Channel { get; set; }
        public string? Type { get; set; }
    }
}
