using Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using OrderService.InfraStructure.Clients;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Xunit;

namespace OrderService.Infrastructure.Tests
{
    public class ServiceTokenClientTests
    {
        [Fact]
        public async Task GetTokenAsync_ShouldRequestToken_WhenCacheIsEmpty()
        {
            // Arrange
            var jwt = CreateJwt(DateTime.UtcNow.AddMinutes(10));

            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, $$"""{"jwtToken":"{{jwt}}"}""");

            using var httpClient = new HttpClient(handler){ BaseAddress = new Uri("https://identity-service/")};

            var configuration = CreateConfiguration();

            var cache = new ServiceTokenCache();

            var client = new ServiceTokenClient(httpClient, configuration, cache);

            // Act
            var result = await client.GetTokenAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(jwt, result);
            Assert.Equal(1, handler.CallCount);
            Assert.Equal(jwt, cache.Token);
        }


        [Fact]
        public async Task GetTokenAsync_ShouldReturnCachedToken_WhenTokenIsStillValid()
        {
            // Arrange
            var jwt = CreateJwt(DateTime.UtcNow.AddMinutes(10));

            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, $$"""{"jwtToken":"{{jwt}}"}""");

            using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://identity-service/") };

            var configuration = CreateConfiguration();

            var cache = new ServiceTokenCache();

            cache.Set(jwt, DateTimeOffset.UtcNow.AddMinutes(10));

            var client = new ServiceTokenClient(httpClient, configuration, cache);

            // Act
            var result = await client.GetTokenAsync( TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(jwt, result);
            Assert.Equal(0, handler.CallCount);
        }


        [Fact]
        public async Task GetTokenAsync_ShouldRequestNewToken_WhenCachedTokenIsExpired()
        {
            // Arrange
            var oldJwt = CreateJwt(DateTime.UtcNow.AddMinutes(-10));
            var newJwt = CreateJwt(DateTime.UtcNow.AddMinutes(10));

            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, $$"""{"jwtToken":"{{newJwt}}"}""");

            using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://identity-service/") };

            var configuration = CreateConfiguration();

            var cache = new ServiceTokenCache();

            cache.Set(oldJwt, DateTimeOffset.UtcNow.AddMinutes(-10));

            var client = new ServiceTokenClient(httpClient, configuration, cache);

            // Act
            var result = await client.GetTokenAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(newJwt, result);
            Assert.Equal(1, handler.CallCount);
            Assert.Equal(newJwt, cache.Token);
        }


        [Fact]
        public async Task GetTokenAsync_ShouldThrow_WhenIdentityServiceFails()
        {
            // Arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.InternalServerError, "");

            using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://identity-service/") };

            var configuration = CreateConfiguration();

            var cache = new ServiceTokenCache();

            var client = new ServiceTokenClient(httpClient, configuration, cache);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => client.GetTokenAsync(TestContext.Current.CancellationToken));
        }


        [Fact]
        public async Task GetTokenAsync_ShouldThrow_WhenIdentityServiceReturnsEmptyToken()
        {
            // Arrange
            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, """{"jwtToken":""}""");

            using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://identity-service/") };

            var configuration = CreateConfiguration();

            var cache = new ServiceTokenCache();

            var client = new ServiceTokenClient(httpClient, configuration, cache);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>( () => client.GetTokenAsync(TestContext.Current.CancellationToken));
        }


        private static IConfiguration CreateConfiguration()
        {
            var values = new Dictionary<string, string?>
            {
                ["OrderService:ServiceId"] = "Order-Service",
                ["OrderService:ServiceSecret"] = "test-secret"
            };

            return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
        }

        private static string CreateJwt(DateTime expires)
        {
            var token = new JwtSecurityToken(
                issuer: "ECommerceLite",
                audience: "ECommerceLite",
                claims: [],
                expires: expires,
                signingCredentials: null);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
