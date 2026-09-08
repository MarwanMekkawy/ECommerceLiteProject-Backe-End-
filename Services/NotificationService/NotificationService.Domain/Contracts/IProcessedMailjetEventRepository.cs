using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Contracts
{
    public interface IProcessedMailjetEventRepository
    {
        Task<bool> ExistsAsync(string mailjetEventId, CancellationToken cancellationToken);
        Task AddAsync(ProcessedMailjetEvent mailjetEvent, CancellationToken cancellationToken);
    }
}
