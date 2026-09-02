using PaymentService.Application.Abstractions;

namespace PaymentService.API.BackgroundTasks
{
    public class RetryCompletingOrRefundingBackgroundService(IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = scopeFactory.CreateScope();

                var retryService = scope.ServiceProvider.GetRequiredService<IRetryCompleteingPayedOrderOrRefundService>();

                await retryService.RetryCompletingOrRefunding(stoppingToken);
                await retryService.RetryCancellingRefundedOrders(stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
