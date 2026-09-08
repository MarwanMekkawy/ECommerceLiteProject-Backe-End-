using NotificationService.Domain.Contracts;

namespace NotificationService.Infrastructure.Repositories
{
    public class UnitOfWork(NotificationServiceDbContext _context, INotificationRepository notificationRepo, IProcessedMailjetEventRepository processedMailjetEventRepo) : IUnitOfWork
    {
        public INotificationRepository NotificationRepo { get; } = notificationRepo;
        public IProcessedMailjetEventRepository ProcessedMailjetEventRepo { get; } = processedMailjetEventRepo;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
