namespace OrderService.InfraStructure.Clients.IdentityServiceTokenAuthClient.DTOIdentityContracts
{
    public class ServiceTokenRequest
    {
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
    }
}
