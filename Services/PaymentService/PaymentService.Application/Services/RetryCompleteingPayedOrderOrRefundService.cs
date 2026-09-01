using PaymentService.Application.Abstractions;
using PaymentService.Domain.Contracts;

namespace PaymentService.Application.Services
{
    public class RetryCompleteingPayedOrderOrRefundService(IPaymentRepository _paymentRepository, IOrderServiceClient _orderServiceClient, IStripePaymentClient _stripePaymentClient) 
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
                        payment.MarkAsRefunding();

                        await _paymentRepository.SaveChangesAsync(cancellationToken);

                        await _stripePaymentClient.CreateRefundAsync(payment.StripePaymentIntentId!, cancellationToken);
                    }
                    else
                    {
                        await _paymentRepository.SaveChangesAsync(cancellationToken);
                    }
                }
            }
        }
    }
}
