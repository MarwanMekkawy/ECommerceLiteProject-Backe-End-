namespace PaymentService.Application.Abstractions.ClientsAbstractions
{
    public interface IServiceTokenClient
    {
        Task<string> GetTokenAsync(CancellationToken cancellationToken = default);
    }
}
