namespace PaymentService.Application.Abstractions
{
    public interface IOrderServiceClient
    {
        Task CompleteOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    }
}
