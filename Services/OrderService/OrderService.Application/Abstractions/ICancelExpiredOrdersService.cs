namespace OrderService.Application.Abstractions
{
    public interface ICancelExpiredOrdersService
    {
        Task CancelExpiredAsync(CancellationToken cancellationToken);
    }
}
