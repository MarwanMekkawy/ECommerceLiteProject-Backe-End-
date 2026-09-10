namespace NotificationService.Application.Abstractions
{
    public interface INotificationDeliveryService
    {
        Task ProcessPendingNotificationsAsync(CancellationToken cancellationToken);
    }
}
