using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Contracts;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Repositories
{
    public class ProcessedMailjetEventRepository(NotificationDbContext _context) : IProcessedMailjetEventRepository
    {
        public async Task<bool> ExistsAsync(string mailjetEventId, CancellationToken cancellationToken)
        {
            return await _context.ProcessedMailjetEvents.AnyAsync(x => x.MailjetEventId == mailjetEventId, cancellationToken);
        }
        public async Task AddAsync(ProcessedMailjetEvent mailjetEvent, CancellationToken cancellationToken)
        {
            await _context.ProcessedMailjetEvents.AddAsync(mailjetEvent, cancellationToken);
        }
    }
}
