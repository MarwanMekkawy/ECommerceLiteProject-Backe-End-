using NotificationService.Domain.Contracts;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Repositories
{
    public class ProcessedMailjetEventRepository : IProcessedMailjetEventRepository
    {
        public Task AddAsync(ProcessedMailjetEvent mailjetEvent, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(string mailjetEventId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
