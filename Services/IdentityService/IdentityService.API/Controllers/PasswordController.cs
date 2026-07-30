using IdentityService.API.CookiesHelpers;
using IdentityService.Application.Abstractions;
using IdentityService.Application.DTOs.PwResetDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers
{
    /// <summary>
    /// Handles password recovery and password reset operations.
    /// </summary>
    [Route("api/v1/password")]
    [ApiController]
    [AllowAnonymous]
    public class PasswordController(IPasswordResetTokenService passwordService) : ControllerBase
    {
        /// <summary>
        /// Generates a password reset token for the specified email address.
        /// </summary>
        /// <param name="dto">The password reset request information.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>Redirects the user after the password reset request is processed.</returns>
        [HttpPost("forgot")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto, CancellationToken cancellationToken)
        {
            var tokenResult = await passwordService.RequestPasswordResetAsync(dto, cancellationToken);

            //@ send email with the token tokenResult:{email,token}

            //@ add valid url to redirect
            //return Redirect("https://myfrontend.com/resendemail");

            //@ for testing
            return Ok(tokenResult);
        }

        /// <summary>
        /// Resets a user's password using a valid password reset token.
        /// </summary>
        /// <param name="token">The password reset token.</param>
        /// <param name="dto">The new password information.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>Redirects the user after the password has been successfully reset.</returns>
        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword(string token, ResetPasswordDto dto, CancellationToken cancellationToken)
        {
            await passwordService.ResetPasswordAndLogOutAllDevicesAsync(token, dto, cancellationToken);

            CookieHelper.DeleteRefreshTokenCookie(Response);

            //@ add valid url to redirect
            //return Redirect("https://myfrontend.com/login");

            //@ for testing
            return Ok();
        }
    }
}
