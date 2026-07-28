using IdentityService.Application.Abstractions;
using IdentityService.Application.DTOs.AuthDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers
{
    /// <summary>
    /// Handles authentication and authorization related operations.
    /// </summary>
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
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

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto, CancellationToken cancellationToken)
        {
            var result = await authService.RegisterAsync(dto, cancellationToken);

            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]  LoginRequestDto dto, CancellationToken cancellationToken)
        {
            var result = await authService.LoginAsync(dto, cancellationToken);

            AppendRefreshTokenCookie(result.RefreshToken);

            return Ok(new { jwtToken = result.AccessToken });
        }

        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken)) return Unauthorized(cancellationToken);

            await authService.LogoutAsync(refreshToken, cancellationToken);

            DeleteRefreshTokenCookie();

            return Ok();
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
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
