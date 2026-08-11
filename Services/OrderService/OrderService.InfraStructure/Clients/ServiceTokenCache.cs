namespace OrderService.InfraStructure.Clients
{
    public class ServiceTokenCache : IServiceTokenCache
    {
        public string? Token { get; private set; }

        public DateTimeOffset? ExpiresAt { get; private set; }

        public void Set(string token, DateTimeOffset expiresAt)
        {
            Token = token;
            ExpiresAt = expiresAt;
        }

        public void Clear()
        {
            Token = null;
            ExpiresAt = null;
        }
    }
}
