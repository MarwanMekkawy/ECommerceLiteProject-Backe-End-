using NotificationService.Domain.Contracts;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        public Task AddAsync(Notification notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Notification>> GetPendingAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
