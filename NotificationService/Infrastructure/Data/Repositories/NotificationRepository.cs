using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Infrastructure.Data.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationsContext _context;

        public NotificationRepository(NotificationsContext context)  
        { 
            _context = context; 
        }

        public async Task AddAsync(Notification notification, CancellationToken ct = default)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<(IReadOnlyList<Notification> Items, int TotalCount)> GetByRecipientAsync(
            Guid recipientId, int page, int pageSize, bool onlyUnread = false, CancellationToken ct = default)
        {
            var query = _context.Notifications
                .Where(n => n.RecipientId == recipientId);

            if (onlyUnread)
                query = query.Where(n => !n.IsRead);

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(ct);

            return (items, total);
        }

        public Task<int> CountUnreadAsync(Guid recipientId, CancellationToken ct = default) =>
            _context.Notifications
               .CountAsync(n => n.RecipientId == recipientId && !n.IsRead, ct);

        public async Task MarkAsReadAsync(
            Guid recipientId, IEnumerable<Guid>? notificationIds, CancellationToken ct = default)
        {
            // Si notificationIds es null → marca TODAS como leídas
            var query = _context.Notifications
                .Where(n => n.RecipientId == recipientId && !n.IsRead);

            if (notificationIds is not null)
                query = query.Where(n => notificationIds.Contains(n.Id));

            await query.ExecuteUpdateAsync(
                s => s.SetProperty(n => n.IsRead, true), ct);
        }
    }

}
