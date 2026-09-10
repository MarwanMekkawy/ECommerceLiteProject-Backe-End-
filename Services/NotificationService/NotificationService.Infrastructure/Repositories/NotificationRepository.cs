using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Contracts;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Enums;

namespace NotificationService.Infrastructure.Repositories
{
    public class NotificationRepository(NotificationDbContext _context) : INotificationRepository
    {
        public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Notifications.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Notification>> GetPendingAsync(CancellationToken cancellationToken)
        {
            return await _context.Notifications.Where(x => x.Status == NotificationStatus.Pending).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Notification>> GetFailedNotificationsDueForRetryAsync(CancellationToken cancellationToken)
        {
            return await _context.Notifications.Where(x => x.Status == NotificationStatus.Failed && x.AttemptCount < 5 && x.NextAttemptAt <= DateTime.UtcNow)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Notification notification, CancellationToken cancellationToken)
        {
            await _context.Notifications.AddAsync(notification, cancellationToken);
        }
    }
}
