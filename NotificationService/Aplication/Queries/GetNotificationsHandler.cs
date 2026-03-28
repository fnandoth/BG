using NotificationService.Aplication.DTOs;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Aplication.Queries
{
    public class GetNotificationsHandler
    {
        private readonly INotificationRepository _repo;

        public GetNotificationsHandler(INotificationRepository repo) => _repo = repo;

        public async Task<PagedResult<NotificationDto>> HandleAsync(
            GetNotificationsQuery query, CancellationToken ct = default)
        {
            var (items, total) = await _repo.GetByRecipientAsync(
                query.RecipientId,
                query.Page,
                query.PageSize,
                query.OnlyUnread,
                ct);

            return new PagedResult<NotificationDto>(
                Items: items.Select(n => n.ToDto()).ToList(),
                Page: query.Page,
                PageSize: query.PageSize,
                TotalCount: total
            );
        }
    }

}
