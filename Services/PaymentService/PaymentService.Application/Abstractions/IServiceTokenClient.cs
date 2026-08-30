namespace PaymentService.Application.Abstractions
{
    public interface IServiceTokenClient
    {
        Task<string> GetTokenAsync(CancellationToken cancellationToken = default);
    }
}
