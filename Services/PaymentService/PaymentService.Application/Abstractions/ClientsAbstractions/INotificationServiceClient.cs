namespace PaymentService.Application.Abstractions.ClientsAbstractions
{
    public interface INotificationServiceClient
    {
        Task SendPaymentFailedAsync(Guid userId, string recipientEmail, string firstName, Guid orderId, Guid paymentId, decimal amount, string currency, string failureReason);
        Task SendPaymentRefundedAsync(Guid userId, string recipientEmail, string firstName, Guid orderId, Guid paymentId, decimal refundAmount);
    }
}
