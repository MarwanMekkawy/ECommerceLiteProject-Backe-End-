using OrderService.Application.Abstractions;

namespace OrderService.API.BackgroundTasks
{
    public class StockCompensationBackgroundService(IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = scopeFactory.CreateScope();

                var compensationService = scope.ServiceProvider.GetRequiredService<IStockCompensationService>();

                await compensationService.CompensateAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
