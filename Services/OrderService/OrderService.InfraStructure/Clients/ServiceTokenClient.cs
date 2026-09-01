using Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using OrderService.Application.Abstractions;
using OrderService.InfraStructure.Clients.DTOIdentityContracts;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Text.Json;

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

            var response = await httpClient.PostAsJsonAsync("auth/oauth/service-token", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                string? error = null;
                if (!string.IsNullOrWhiteSpace(json))
                {
                    using var document = JsonDocument.Parse(json);

                    if (document.RootElement.TryGetProperty("error", out var errorProperty)) error = errorProperty.GetString();
                }
                throw new HttpRequestException(error ?? $"Identity Service returned {(int)response.StatusCode}.");
            }

            var result = await response.Content.ReadFromJsonAsync<ServiceTokenResponse>(cancellationToken);

            if (string.IsNullOrWhiteSpace(result?.JwtToken))
                throw new UnauthorizedException("IdentityService returned an empty service JWT.");

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.JwtToken);

            var expiresAt = jwt.ValidTo;

            cache.Set(result.JwtToken,new DateTimeOffset(expiresAt));

            return result.JwtToken;
        }
    }
}
