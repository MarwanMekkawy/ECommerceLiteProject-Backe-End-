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
    /// Handles authenticated user profile management operations.
    /// </summary>
    [Route("api/v1/users")]
    [ApiController]
    [Authorize]
    public class UsersController(IUserService userService) : ControllerBase
    {
        /// <summary>
        /// Retrieves the profile of the currently authenticated user.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>The current user's profile information.</returns>
        [HttpGet("me")]
        public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            var result = await userService.GetCurrentUserAsync(claims.UserId, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Updates the profile information of the currently authenticated user.
        /// </summary>
        /// <param name="dto">The updated profile information.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the profile was updated successfully.</returns>
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            await userService.UpdateProfileAsync(claims.UserId, dto, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Deactivates the currently authenticated user's account.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the account was deactivated successfully.</returns>
        [HttpPatch("me")]
        public async Task<IActionResult> DeactivateMe(CancellationToken cancellationToken)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            await userService.DeactivateAccountAsync(claims.UserId, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Changes the password of the currently authenticated user.
        /// </summary>
        /// <param name="dto">The current and new password information.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the password was changed successfully.</returns>
        [HttpPost("me/change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto, CancellationToken cancellationToken)
        {
            var claims = UserClaimsFactory.ExtractFrom(User);

            await userService.ChangePasswordAsync(claims.UserId, dto, cancellationToken);

            return NoContent();
        }
    }
}
