namespace OrderService.Application.Abstractions
{
    public interface IServiceTokenClient
    {
        Task<string> GetTokenAsync(CancellationToken cancellationToken = default);
    }
}
