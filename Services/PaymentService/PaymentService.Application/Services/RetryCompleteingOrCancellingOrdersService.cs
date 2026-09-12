using PaymentService.Application.Abstractions;
using PaymentService.Application.Abstractions.ClientsAbstractions;
using PaymentService.Domain.Contracts;

namespace PaymentService.Application.Services
{
    public class RetryCompleteingOrCancellingOrdersService(IPaymentRepository _paymentRepository, IOrderServiceClient _orderServiceClient, IStripePaymentClient _stripePaymentClient) 
        : IRetryCompleteingPayedOrderOrRefundService
    {
        public async Task RetryCompletingOrRefunding(CancellationToken cancellationToken)
        {
            var payments = await _paymentRepository.GetSucceededPaymentsWithUnconfirmedOrderAsync(cancellationToken);

            foreach (var payment in payments)
            {
                try
                {
                    payment.AttemptToComplete();
                    await _orderServiceClient.CompleteOrderAsync(payment.OrderId, cancellationToken);

                    payment.MarkCompletionConfirmed();
                    await _paymentRepository.SaveChangesAsync(cancellationToken);
                }
                catch (Exception)
                {
                    if (payment.OrderCompletionReattempts >= 5)
                    {
                        payment.MarkAsRefundInitiated();
                        await _paymentRepository.SaveChangesAsync(cancellationToken);

                        var refundId = await _stripePaymentClient.CreateRefundAsync(payment.StripePaymentIntentId!, cancellationToken);

                        payment.SetStripeRefundId(refundId);
                        await _paymentRepository.SaveChangesAsync(cancellationToken);
                    }
                    else
                    {
                        await _paymentRepository.SaveChangesAsync(cancellationToken);
                    }
                }
            }
        }
        public async Task RetryCancellingRefundedOrders(CancellationToken cancellationToken)
        {
            var payments = await _paymentRepository.GetRefundedPaymentsWithUnconfirmedOrderCancellationAsync(cancellationToken);

            foreach (var payment in payments)
            {
                try
                {
                    payment.AttemptToCancelRefundedOrder();
                    await _orderServiceClient.CancelRefundedOrderAsync(payment.OrderId, cancellationToken);

                    payment.MarkCancellationDueToRefundConfirmed();
                    await _paymentRepository.SaveChangesAsync(cancellationToken);
                }
                catch (Exception)
                {
                    await _paymentRepository.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }
}
