namespace NotificationService.Application.Abstractions
{
    public interface IRetrySendingFailedMailsService
    {
        Task RetryFailedNotificationsAsync(CancellationToken cancellationToken);
    }
}
