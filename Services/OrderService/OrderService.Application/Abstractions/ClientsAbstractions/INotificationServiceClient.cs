namespace OrderService.Application.Abstractions.ClientsAbstractions
{
    public interface INotificationServiceClient
    {
        Task SendOrderConfirmedAsync(Guid userId, string recipientEmail, string firstName, Guid orderId, decimal total, string currency, DateTime paymentExpiresAt, CancellationToken cancellationToken);
        Task SendOrderCompletedAsync(Guid userId, string recipientEmail, string firstName, Guid orderId, object items, decimal total, string currency, CancellationToken cancellationToken);
        Task SendOrderCancelledAsync(Guid userId, string recipientEmail, string firstName, Guid orderId, string reason, CancellationToken cancellationToken);
        Task SendOrderExpiredAsync(Guid userId, string recipientEmail, string firstName, Guid orderId, CancellationToken cancellationToken);
    }
}
