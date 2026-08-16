using Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using OrderService.InfraStructure.Clients;
using System.Net;
using Xunit;

namespace OrderService.Infrastructure.Tests
{
    public class ServiceTokenClientTests
    {
        [Fact]
        public async Task GetTokenAsync_ShouldReturnCachedToken_WhenTokenIsStillValid()
        {
            // Arrange
            var jwt = "cached-service-token";

            var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, "");

            using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://identity-service/") };

            var configuration = CreateConfiguration();

            var cache = new ServiceTokenCache();

            cache.Set(jwt, DateTimeOffset.UtcNow.AddHours(1));

            var client = new ServiceTokenClient(httpClient, configuration, cache);

            // Act
            var result = await client.GetTokenAsync(TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(jwt, result);
            Assert.Equal(0, handler.CallCount);
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
            await Assert.ThrowsAsync<UnauthorizedException>(() => client.GetTokenAsync(TestContext.Current.CancellationToken));
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
    }
}
