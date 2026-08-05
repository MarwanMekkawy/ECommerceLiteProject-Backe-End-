

namespace IdentityService.Application.DTOs
{
    public class ServiceTokenRequestDto
    {
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
    }
}
