namespace OrderService.Application.Abstractions
{
    public interface IStockCompensationService
    {
        Task CompensateAsync(CancellationToken cancellationToken);
    }
}
