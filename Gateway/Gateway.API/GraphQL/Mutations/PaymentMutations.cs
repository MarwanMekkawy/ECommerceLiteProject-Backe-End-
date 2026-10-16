using Gateway.API.Abstractions;
using Gateway.API.DTOs;
namespace Gateway.API.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class PaymentMutations
    {
        public Task<RetryPaymentResponseDto> RetryPaymentAsync(Guid orderId, IPaymentServiceClient paymentServiceClient, CancellationToken cancellationToken)
            => paymentServiceClient.RetryPaymentAsync(orderId, cancellationToken);

        public async Task<RefundPaymentPayload> RefundPaymentAsync(Guid paymentId, IPaymentServiceClient paymentServiceClient, CancellationToken cancellationToken)
        {
            await paymentServiceClient.RefundPaymentAsync(paymentId, cancellationToken);

            return new RefundPaymentPayload(paymentId, true);
        }
    }
    public sealed record RefundPaymentPayload(Guid PaymentId, bool Success);
}
