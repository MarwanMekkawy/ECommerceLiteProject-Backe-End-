using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Contracts
{
    public interface INotificationRepository
    {
        Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<Notification>> GetPendingAsync(CancellationToken cancellationToken);
        Task AddAsync(Notification notification, CancellationToken cancellationToken);
    }
}
