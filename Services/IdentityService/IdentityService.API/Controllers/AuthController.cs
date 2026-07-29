using IdentityService.Application.Abstractions;
using IdentityService.Application.DTOs.AuthDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers
{
    /// <summary>
    /// Handles authentication and session management operations.
    /// </summary>
    [Route("api/v1/auth")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController(IAuthService authService, IEmailVerificationTokenService emailVerification) : ControllerBase
    {
        #region// Cookie helper methods ================================================================

        // Storing refresh token in HttpOnly cookie
        private void AppendRefreshTokenCookie(string refreshToken)
        {
            Response.Cookies.Append("refreshToken", refreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Path = "/api/v1/auth",
                    Expires = DateTimeOffset.UtcNow.AddDays(15)
                });
        }

        // Removes the refresh token cookie
        private void DeleteRefreshTokenCookie()
        {
            Response.Cookies.Delete("refreshToken", new CookieOptions { Path = "/api/v1/auth" });
        }

        #endregion =====================================================================================

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="dto">The registration information.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>The newly created user information.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto, CancellationToken cancellationToken)
        {
            var registerResultUserId = await authService.RegisterAsync(dto, cancellationToken);

            var emailTokenResult = await emailVerification.GenerateVerificationTokenAsync(registerResultUserId.userId, cancellationToken);

            //@ generate email confirm token and call endpoint to send email with it 

            //@ for testing
            return Ok(new { registerResultUserId, emailTokenResult });
        }

        /// <summary>
        /// Authenticates a user and creates a new session.
        /// </summary>
        /// <param name="dto">The user's login credentials.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>A JWT access token and a refresh token stored in an HttpOnly cookie.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto, CancellationToken cancellationToken)
        {
            var result = await authService.LoginAsync(dto, cancellationToken);

            AppendRefreshTokenCookie(result.RefreshToken);

            return Ok(new { jwtToken = result.AccessToken });
        }

        /// <summary>
        /// Logs out the current user and invalidates the active refresh token.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the user was logged out successfully.</returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken)) return Unauthorized(cancellationToken);

            await authService.LogoutAsync(refreshToken, cancellationToken);

            DeleteRefreshTokenCookie();

            return NoContent();
        }

        /// <summary>
        /// Refreshes the current session using the refresh token stored in the HttpOnly cookie.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>A new JWT access token and a rotated refresh token.</returns>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrWhiteSpace(refreshToken)) return Unauthorized();

            var result = await authService.RefreshSessionAsync(refreshToken, cancellationToken);

            AppendRefreshTokenCookie(result.RefreshToken);

            return Ok(new { jwtToken = result.AccessToken });
        }
    }
}
