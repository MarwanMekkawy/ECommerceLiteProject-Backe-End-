namespace PaymentService.Application.Abstractions
{
    public interface IRetryCompleteingPayedOrderOrRefundService 
    {
        Task RetryCompletingOrRefunding(CancellationToken cancellationToken);
        Task RetryCancellingRefundedOrders(CancellationToken cancellationToken);
    }
}
