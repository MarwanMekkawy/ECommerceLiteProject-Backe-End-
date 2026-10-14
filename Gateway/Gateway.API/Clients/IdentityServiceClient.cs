using Gateway.API.Abstractions;
using Gateway.API.DTOs;
using Gateway.API.Middleware;

namespace Gateway.API.Clients
{
    public class IdentityServiceClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor) : IIdentityServiceClient
    {
        #region cosntructing the requests[helpers]==================================================================================================================
        private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string endpoint, object? request = null, CancellationToken cancellationToken = default)
        {
            using var message = new HttpRequestMessage(method, endpoint);

            if (request is not null)
                message.Content = JsonContent.Create(request);

            var context = httpContextAccessor.HttpContext;

            if (context is not null)
            {
                if (context.Request.Headers.TryGetValue("Authorization", out var authorization))
                    message.Headers.TryAddWithoutValidation("Authorization", authorization.ToString());

                if (context.Request.Headers.TryGetValue("Cookie", out var cookie))
                    message.Headers.TryAddWithoutValidation("Cookie", cookie.ToString());
            }

            var response = await httpClient.SendAsync(message, cancellationToken);

            if (context is not null && response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                foreach (var cookie in cookies)
                    context.Response.Headers.Append("Set-Cookie", cookie);
            }

            if (!response.IsSuccessStatusCode)
            {
                var statusCode = (int)response.StatusCode;
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                response.Dispose();
                throw new DownstreamServiceException(statusCode, responseBody);
            }

            return response;
        }

        private async Task SendNoContentAsync(HttpMethod method, string endpoint, object? request = null, CancellationToken cancellationToken = default)
        {
            using var response = await SendAsync(method, endpoint, request, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        private async Task<T> SendForResultAsync<T>(HttpMethod method, string endpoint, object? request = null, CancellationToken cancellationToken = default)
        {
            using var response = await SendAsync(method, endpoint, request, cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);

            return result ?? throw new InvalidOperationException($"IdentityService returned an empty response for {endpoint}.");
        }
        #endregion =================================================================================================================================================

        // Authentication

        public async Task RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
        {
            await SendNoContentAsync(HttpMethod.Post, "auth/register", request, cancellationToken);
        }

        public async Task<JwtResponsetDto> LoginAsync( LoginRequestDto request,CancellationToken cancellationToken = default)
        {
            return await SendForResultAsync<JwtResponsetDto>(HttpMethod.Post, "auth/login", request, cancellationToken);
        }

        public async Task LogoutAsync(CancellationToken cancellationToken = default)
        {
            await SendNoContentAsync(HttpMethod.Post, "auth/logout", cancellationToken: cancellationToken);
        }

        public async Task<JwtResponsetDto> RefreshTokenAsync(CancellationToken cancellationToken = default)
        {
            return await SendForResultAsync<JwtResponsetDto>(HttpMethod.Post, "auth/refresh", cancellationToken: cancellationToken);
        }

        // Email

        public async Task ResendVerificationAsync(CancellationToken cancellationToken = default)
        {
            await SendNoContentAsync(HttpMethod.Post, "email/resend-verification", cancellationToken: cancellationToken);
        }

        public async Task RequestEmailChangeAsync(ChangeEmailRequestDto request, CancellationToken cancellationToken = default)
        {
            await SendNoContentAsync(HttpMethod.Post, "email/change-request", request, cancellationToken);
        }

        // Password

        public async Task ForgotPasswordAsync( ForgotPasswordRequestDto request,   CancellationToken cancellationToken = default)
        {
            await SendNoContentAsync(HttpMethod.Post, "password/forgot", request, cancellationToken);
        }

        public async Task ResetPasswordAsync( ResetPasswordRequestDto request, CancellationToken cancellationToken = default)
        {
            var context = httpContextAccessor.HttpContext;

            var token = context?.Request.Query["token"].ToString();

            if (string.IsNullOrWhiteSpace(token))           
                throw new InvalidOperationException("The password reset token is missing from the incoming request query.");

            await SendNoContentAsync(HttpMethod.Post, $"password/reset?token={Uri.EscapeDataString(token)}", request, cancellationToken);
        }

        // Current user
        public async Task<UserDto> GetCurrentUserAsync(CancellationToken cancellationToken = default)
        {
            return await SendForResultAsync<UserDto>(HttpMethod.Get, "users/me", cancellationToken: cancellationToken);
        }

        public async Task UpdateCurrentUserAsync(UpdateUserRequestDto request, CancellationToken cancellationToken = default)
        {
            await SendNoContentAsync(HttpMethod.Put, "users/me", request, cancellationToken);
        }

        public async Task DeactivateCurrentUserAsync(CancellationToken cancellationToken = default)
        {
            await SendNoContentAsync(HttpMethod.Patch, "users/me", cancellationToken: cancellationToken);
        }

        public async Task ChangePasswordAsync(ChangePasswordRequestDto request, CancellationToken cancellationToken = default)
        {
            await SendNoContentAsync(HttpMethod.Post, "users/me/change-password", request, cancellationToken);
        }

        // Admin
        public async Task<PagedResult<UserDto>> GetUsersAsync(PaginationQueryDto query, CancellationToken cancellationToken = default)
        {
            var endpoint = $"admin/users?status=All&pageNumber={query.pageNumber}&pageSize={query.pageSize}";

            return await SendForResultAsync<PagedResult<UserDto>>(HttpMethod.Get, endpoint, cancellationToken: cancellationToken);
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await SendForResultAsync<UserDto>(HttpMethod.Get, $"admin/users/{id}", cancellationToken: cancellationToken);
        }

        public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await SendNoContentAsync(HttpMethod.Delete, $"admin/users/{id}", cancellationToken: cancellationToken);
        }

        public async Task ChangeUserRoleAsync(Guid id, ChangeUserRoleRequestDto request, CancellationToken cancellationToken = default)
        {
            await SendNoContentAsync(HttpMethod.Patch, $"admin/users/{id}/role", request, cancellationToken);
        }

        public async Task ActivateUserAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await SendNoContentAsync(HttpMethod.Patch, $"admin/users/{id}/activate", cancellationToken: cancellationToken);
        }

        public async Task DeactivateUserAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await SendNoContentAsync(HttpMethod.Patch, $"admin/users/{id}/deactivate", cancellationToken: cancellationToken);
        }
    }
}
