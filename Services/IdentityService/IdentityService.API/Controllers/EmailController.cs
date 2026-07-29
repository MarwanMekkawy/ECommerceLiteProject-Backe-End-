using IdentityService.API.ApiClaimsFactory;
using IdentityService.Application.Abstractions;
using IdentityService.Application.DTOs.EmailVerificationDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers
{
    /// <summary>
    /// Handles Email confirmation and changing.
    /// </summary>
    [Route("api/v1/email")]
    [ApiController]
    [Authorize]
    public class EmailController(IEmailVerificationTokenService emailVerificationService) : ControllerBase
    {
        [HttpGet("confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token, CancellationToken cancellationToken)
        {
            await emailVerificationService.ConfirmEmailAsync(token, cancellationToken);

            //@ add valid url to redirect
            return Redirect("https://myfrontend.com/login");
        }

        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendEmailVerificationToken(CancellationToken cancellationToken)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var result = await emailVerificationService.ResendVerificationEmailAsync(claims.UserId, cancellationToken);
          
            //@ call api mail service to send the token [ result ] in email

            return NoContent();
        }

        [HttpPost("change-request")]
        public async Task<IActionResult> ChangeEmail(ChangeEmailRequestDto dto,CancellationToken cancellationToken)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var tokenResult = await emailVerificationService.GenerateEmailChangeTokenAsync(claims.UserId, dto, cancellationToken);

            //@ extract new email from dto and mail the token

            //@ add valid url to redirect
            return Redirect("https://myfrontend.com/resendemail");
        }

        [HttpPost("confirm-change")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailChange([FromQuery] string token, CancellationToken cancellationToken)
        {
            await emailVerificationService.ConfirmEmailChangeAsync(token, cancellationToken);

            //@ add valid url to redirect
            return Redirect("https://myfrontend.com/login");
        }
    }
}
