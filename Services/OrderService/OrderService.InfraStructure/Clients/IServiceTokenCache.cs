namespace OrderService.InfraStructure.Clients
{
    public interface IServiceTokenCache
    {
        string? Token { get; }
        DateTimeOffset? ExpiresAt { get; }

        void Set(string token, DateTimeOffset expiresAt);

        void Clear();
    }
}
