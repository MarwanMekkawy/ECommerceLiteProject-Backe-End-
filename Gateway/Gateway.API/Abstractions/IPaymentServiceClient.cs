using Gateway.API.DTOs;

namespace Gateway.API.Abstractions
{
    public interface IPaymentServiceClient
    {
        Task<RetryPaymentResponseDto> RetryPaymentAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task RefundPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default);
    }
}
