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
    /// Exposes endpoints for managing user profile.
    /// </summary>
    [Route("api/v1/users")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var result = await userService.GetCurrentUserAsync(claims.UserId, cancellationToken);

            return Ok(result);
        }

        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            await userService.UpdateProfileAsync(claims.UserId, dto, cancellationToken);

            return Ok();
        }

        [HttpPatch("me")]
        [Authorize]
        public async Task<IActionResult> DeactivateMe(CancellationToken cancellationToken) 
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            await userService.DeactivateAccountAsync(claims.UserId, cancellationToken);
            
            return NoContent();
        }
    }
}
