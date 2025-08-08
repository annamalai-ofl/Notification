using Microsoft.AspNetCore.Mvc;
using Notification.Shared;
using Notification.Producer.Services;
using System.Threading.Tasks;

namespace Notification.Producer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] NotificationMessage notification)
        {
            var result = await _notificationService.SendNotificationAsync(notification);
            if (!result)
                return BadRequest($"Notification too large for the batch: {notification.Title}");
            return Ok($"Notification sent to Event Hub: {notification.Title}");
        }

        [HttpPost("sendbatch")]
        public async Task<IActionResult> SendBatch([FromBody] NotificationMessage[] notifications)
        {
            var result = await _notificationService.SendNotificationsAsync(notifications);
            if (!result)
                return BadRequest("One or more notifications too large for the batch.");
            return Ok($"Sent {notifications.Length} notifications to Event Hub.");
        }
    }
}
