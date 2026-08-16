namespace OrderService.InfraStructure.Clients.DTOIdentityContracts
{
    public class ServiceTokenRequest
    {
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
    }
}
