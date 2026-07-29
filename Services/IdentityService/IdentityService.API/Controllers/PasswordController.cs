using IdentityService.Application.Abstractions;
using IdentityService.Application.DTOs.PwResetDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers
{
    /// <summary>
    /// Handles forgetting password reset.
    /// </summary>
    [Route("api/v1/password")]
    [ApiController]
    [AllowAnonymous]
    public class PasswordController(IPasswordResetTokenService passwordService) : ControllerBase
    {
        [HttpPost("forgot")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto, CancellationToken cancellationToken)
        {
            var tokenResult = await passwordService.RequestPasswordResetAsync(dto, cancellationToken);

            //@ send email with the token tokenResult:{email,token}

            //@ add valid url to redirect
            return Redirect("https://myfrontend.com/resendemail");
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword(string token,ResetPasswordDto dto, CancellationToken cancellationToken)
        {
            await passwordService.ResetPasswordAsync(token, dto, cancellationToken);


            //@ add valid url to redirect
            return Redirect("https://myfrontend.com/login");
        }
    }
}
