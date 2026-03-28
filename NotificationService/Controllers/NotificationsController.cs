using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Aplication.Commands;
using NotificationService.Aplication.Queries;

namespace NotificationService.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly GetNotificationsHandler _getHandler;
        private readonly MarkAsReadHandler _markHandler;

        public NotificationsController(
            GetNotificationsHandler getHandler,
            MarkAsReadHandler markHandler)
        {
            _getHandler = getHandler;
            _markHandler = markHandler;
        }

        // GET /api/notifications?page=1&pageSize=20&onlyUnread=false
        [HttpGet]
        public async Task<IActionResult> GetNotifications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] bool onlyUnread = false,
            CancellationToken ct = default)
        {
            var recipientId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await _getHandler.HandleAsync(
                new GetNotificationsQuery(recipientId, page, pageSize, onlyUnread), ct);

            return Ok(result);
        }

        // PATCH /api/notifications/read
        // Body: { "notificationIds": [ "guid1", "guid2", ... ] } marca la lista, si notificationIds es null, marca todas las notificaciones como leídas
        [HttpPatch("read")]
        public async Task<IActionResult> MarkAsRead(
            [FromBody] MarkAsReadRequest? request,
            CancellationToken ct = default)
        {
            var recipientId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _markHandler.HandleAsync(
                new MarkAsReadCommand(recipientId, request?.NotificationIds), ct);

            return NoContent();
        }
    }

    public record MarkAsReadRequest(IEnumerable<Guid>? NotificationIds);
}
