using IdentityService.API.ApiClaimsFactory;
using IdentityService.Application.Abstractions;
using IdentityService.Application.DTOs.UserDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentityService.API.Controllers
{
    /// <summary>
    /// Handles managing user profile.
    /// </summary>
    [Route("api/v1/users")]
    [ApiController]
    [Authorize]
    public class UsersController(IUserService userService) : ControllerBase
    {
        [HttpGet("me")]    
        public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var result = await userService.GetCurrentUserAsync(claims.UserId, cancellationToken);

            return Ok(result);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            await userService.UpdateProfileAsync(claims.UserId, dto, cancellationToken);

            return NoContent();
        }

        [HttpPatch("me")]
        public async Task<IActionResult> DeactivateMe(CancellationToken cancellationToken) 
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            await userService.DeactivateAccountAsync(claims.UserId, cancellationToken);
            
            return NoContent();
        }

        [HttpPost("me/change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto,CancellationToken cancellationToken)////////////////////////////////
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            await userService.ChangePasswordAsync(claims.UserId, dto, cancellationToken);

            return NoContent();
        }
    }
}
