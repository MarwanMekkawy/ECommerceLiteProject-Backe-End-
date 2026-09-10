namespace NotificationService.Domain.Contracts
{
    public interface IUnitOfWork
    {
        INotificationRepository NotificationRepo { get; }
        IProcessedMailjetEventRepository ProcessedMailjetEventRepo { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
