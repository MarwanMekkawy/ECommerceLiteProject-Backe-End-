using IdentityService.API.ApiClaimsFactory;
using IdentityService.Application.Abstractions;
using IdentityService.Application.DTOs.EmailVerificationDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers
{
    /// <summary>
    /// Handles email verification and email address change operations.
    /// </summary>
    [Route("api/v1/email")]
    [ApiController]
    [Authorize]
    public class EmailController(IEmailVerificationTokenService emailVerificationService) : ControllerBase
    {
        /// <summary>
        /// Confirms a user's email address using a verification token.
        /// </summary>
        /// <param name="token">The email verification token.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>Redirects the user to the frontend after successful email confirmation.</returns>
        [HttpGet("confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token, CancellationToken cancellationToken)
        {
            await emailVerificationService.ConfirmEmailAsync(token, cancellationToken);

            //@ add valid url to redirect
            //return Redirect("https://myfrontend.com/login");

            //@ for testing
            return Ok("confirmed");
        }

        /// <summary>
        /// Generates and sends a new email verification token for the authenticated user.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the verification email was successfully requested.</returns>
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendEmailVerificationToken(CancellationToken cancellationToken)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var result = await emailVerificationService.ResendVerificationEmailAsync(claims.UserId, cancellationToken);

            //@ call api mail service to send the token [ result ] in email

            return NoContent();
        }

        /// <summary>
        /// Initiates the email address change process by generating a verification token for the new email address.
        /// </summary>
        /// <param name="dto">The requested email change information.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>Redirects the user after the email change request is created.</returns>
        [HttpPost("change-request")]
        [Authorize(Policy = "VerifiedEmail")]
        public async Task<IActionResult> ChangeEmail(ChangeEmailRequestDto dto, CancellationToken cancellationToken)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var tokenResult = await emailVerificationService.GenerateEmailChangeTokenAsync(claims.UserId, dto, cancellationToken);

            //@ extract new email from dto and mail the token

            //@ add valid url to redirect
            //return Redirect("https://myfrontend.com/resendemail");

            //@ for testing
            return Ok(tokenResult);
        }

        /// <summary>
        /// Confirms an email address change using the provided verification token.
        /// </summary>
        /// <param name="token">The email change verification token.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>Redirects the user to the frontend after the email address has been updated.</returns>
        [HttpPost("confirm-change")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailChange([FromQuery] string token, CancellationToken cancellationToken)
        {
            await emailVerificationService.ConfirmEmailChangeAsync(token, cancellationToken);

            //@ add valid url to redirect
            //return Redirect("https://myfrontend.com/login");

            //@ for testing
            return Ok();
        }
    }
}
