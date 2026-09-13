using Domain.Exceptions;
using IdentityService.Application.Abstractions.ClientsAbstractions;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace IdentityService.Infrastructure.Clients.NotificationServiceClient
{
    public class NotificationServiceClient(HttpClient httpClient, ISelfServiceClientService selfServiceClientService, IServiceTokenCache cache)  : INotificationServiceClient
    {
        private async Task SendAsync(SendEmailNotificationRequestDto request, string token)
        {
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "notifications");

            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            httpRequest.Content = JsonContent.Create(request);

            using var response = await httpClient.SendAsync(
                httpRequest,
                HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
            {
                string? error = null;
                try
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var document = JsonDocument.Parse(json);
                    error = document.RootElement.GetProperty("error").GetString();
                }
                catch
                {
                }

                throw new Exception(error ?? $"NotificationService returned {(int)response.StatusCode} {response.StatusCode}");
            }
        }

        private async Task<string> GetTokenAsync(CancellationToken cancellationToken)
        {
            if (cache.Token is not null && cache.ExpiresAt is not null && cache.ExpiresAt > DateTimeOffset.UtcNow.AddSeconds(30))
            {
                return cache.Token;
            }           
           
            var token = await selfServiceClientService.SelfAuthinticateAsync(cancellationToken);

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            var expiresAt = jwt.ValidTo;

            cache.Set(token, new DateTimeOffset(expiresAt));

            return token;
        }

        public async Task SendEmailConfirmationAsync(Guid userId, string recipientEmail, string firstName, string confirmationUrl, CancellationToken cancellationToken)
        {
            var token = await GetTokenAsync(cancellationToken);

            var request = new SendEmailNotificationRequestDto
            {
                UserId = userId,
                RecipientEmail = recipientEmail,
                Type = NotificationType.EmailConfirmation,
                Data = new Dictionary<string, object>
                {
                    ["firstName"] = firstName,
                    ["confirmationUrl"] = confirmationUrl
                }
            };

            await SendAsync(request, token);
        }

        public async Task SendEmailChangedAsync(Guid userId, string recipientEmail, string firstName, string newEmail,CancellationToken cancellationToken)
        {
            var token = await GetTokenAsync(cancellationToken);

            var request = new SendEmailNotificationRequestDto
            {
                UserId = userId,
                RecipientEmail = recipientEmail,
                Type = NotificationType.EmailChanged,
                Data = new Dictionary<string, object>
                {
                    ["firstName"] = firstName,
                    ["newEmail"] = newEmail
                }
            };

            await SendAsync(request, token);
        }

        public async Task SendPasswordResetAsync(Guid userId, string recipientEmail, string firstName, string resetUrl, int expirationMinutes, CancellationToken cancellationToken)
        {
            var token = await GetTokenAsync(cancellationToken);

            var request = new SendEmailNotificationRequestDto
            {
                UserId = userId,
                RecipientEmail = recipientEmail,
                Type = NotificationType.PasswordReset,
                Data = new Dictionary<string, object>
                {
                    ["firstName"] = firstName,
                    ["resetUrl"] = resetUrl,
                    ["expirationMinutes"] = expirationMinutes
                }
            };

            await SendAsync(request, token);
        }

        public async Task SendPasswordChangedAsync(Guid userId, string recipientEmail, string firstName, CancellationToken cancellationToken)
        {
            var token = await GetTokenAsync(cancellationToken);

            var request = new SendEmailNotificationRequestDto
            {
                UserId = userId,
                RecipientEmail = recipientEmail,
                Type = NotificationType.PasswordChanged,
                Data = new Dictionary<string, object>
                {
                    ["firstName"] = firstName
                }
            };

            await SendAsync(request, token);
        }
    }
}
