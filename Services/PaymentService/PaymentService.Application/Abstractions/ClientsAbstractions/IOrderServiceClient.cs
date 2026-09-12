namespace PaymentService.Application.Abstractions.ClientsAbstractions
{
    public interface IOrderServiceClient
    {
        Task CompleteOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task CancelRefundedOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    }
}
