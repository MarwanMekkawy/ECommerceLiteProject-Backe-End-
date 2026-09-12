namespace OrderService.Application.Abstractions.ClientsAbstractions
{
    public interface INotificationServiceClient
    {
        Task SendOrderConfirmedAsync(Guid userId, string recipientEmail, string firstName, Guid orderId, decimal total, string currency, DateTime paymentExpiresAt);
        Task SendOrderCompletedAsync(Guid userId, string recipientEmail, string firstName, Guid orderId, object items, decimal total, string currency);
        Task SendOrderCancelledAsync(Guid userId, string recipientEmail, string firstName, Guid orderId, string reason);
        Task SendOrderExpiredAsync(Guid userId, string recipientEmail, string firstName, Guid orderId);
    }
}
