using System.Security.Cryptography;
using System.Text;

namespace NotificationService.API.Middlewere
{
    public class MailjetWebhookAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/v1/notifications/webhook"))
            {
                var username = configuration["Mailjet:MailjetWebhook:Username"];
                var password = configuration["Mailjet:MailjetWebhook:Password"];

                if (!context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader) ||
                    !authorizationHeader.ToString().StartsWith("Basic "))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                var encodedCredentials = authorizationHeader.ToString()["Basic ".Length..];

                string credentials;

                try
                {
                    credentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
                }
                catch
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                var separatorIndex = credentials.IndexOf(':');

                if (separatorIndex < 0)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                var requestUsername = credentials[..separatorIndex];
                var requestPassword = credentials[(separatorIndex + 1)..];

                if (requestUsername != username || !CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(requestPassword), Encoding.UTF8.GetBytes(password ?? string.Empty))) 
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
            }

            await next(context);
        }
    }
}
