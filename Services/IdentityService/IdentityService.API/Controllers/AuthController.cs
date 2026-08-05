using IdentityService.API.CookiesHelpers;
using IdentityService.Application.Abstractions;
using IdentityService.Application.DTOs;
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
    public class AuthController(IAuthService authService, IServiceClientService clientService, IEmailVerificationTokenService emailVerification)
        : ControllerBase
    {
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

            var emailVerificationTokenResult = await emailVerification.GenerateVerificationTokenAsync(registerResultUserId.userId, cancellationToken);

            //@ generate email confirm token and call endpoint to send email with it 

            //@ for testing
            return Ok(new { registerResultUserId, emailVerificationTokenResult });
        }

        /// <summary>
        /// Authenticates a user and creates a new session if email confirmed.
        /// </summary>
        /// <param name="dto">The user's login credentials.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>A JWT access token and a refresh token stored in an HttpOnly cookie.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto, CancellationToken cancellationToken)
        {
            var result = await authService.LoginAsync(dto, cancellationToken);

            CookieHelper.AppendRefreshTokenCookie(Response, result.RefreshToken);

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

            if (string.IsNullOrEmpty(refreshToken)) return Unauthorized(new { message = "you are are logged out." });

            await authService.LogoutAsync(refreshToken, cancellationToken);

            CookieHelper.DeleteRefreshTokenCookie(Response);

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

            if (string.IsNullOrWhiteSpace(refreshToken)) return Unauthorized(new { message = "you are are logged out." });

            var result = await authService.RefreshSessionAsync(refreshToken, cancellationToken);

            CookieHelper.AppendRefreshTokenCookie(Response, result.RefreshToken);

            return Ok(new { jwtToken = result.AccessToken });
        }

        /// <summary>
        /// Authenticates a microservice.
        /// </summary>
        /// <param name="dto">The serviceClient credentials.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>A ServiceClient JWT access token.</returns>
        [HttpPost("oauth/service-token")]
        public async Task<IActionResult> ServiceToken([FromBody] ServiceTokenRequestDto dto, CancellationToken cancellationToken)
        {
            var result = await clientService.AuthinticateAsync(dto.ClientId, dto.ClientSecret, cancellationToken);

            return Ok(new { jwtToken = result.AccessToken });
        }
    }
}
