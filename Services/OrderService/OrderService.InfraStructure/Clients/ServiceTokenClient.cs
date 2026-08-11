using Microsoft.Extensions.Configuration;
using OrderService.Application.Abstractions;
using OrderService.InfraStructure.Clients.DTOIdentityContracts;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;

namespace OrderService.InfraStructure.Clients
{
    public class ServiceTokenClient(HttpClient httpClient, IConfiguration configuration, IServiceTokenCache cache) : IServiceTokenClient
    {
        public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            if (cache.Token is not null && cache.ExpiresAt is not null && cache.ExpiresAt > DateTimeOffset.UtcNow.AddSeconds(30))
            {
                return cache.Token;
            }

            var request = new ServiceTokenRequest
            {
                ClientId = configuration["OrderService:ServiceId"]!,
                ClientSecret = configuration["OrderService:ServiceSecret"]!
            };

            var response = await httpClient.PostAsJsonAsync("oauth/service-token", request, cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<ServiceTokenResponse>(cancellationToken);

            if (string.IsNullOrWhiteSpace(result?.JwtToken))
                throw new InvalidOperationException("IdentityService returned an empty service JWT.");

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.JwtToken);

            var expiresAt = jwt.ValidTo;

            cache.Set(result.JwtToken,new DateTimeOffset(expiresAt));

            return result.JwtToken;
        }
    }
}
