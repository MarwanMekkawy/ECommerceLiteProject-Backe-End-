using IdentityService.Application.Abstractions;
using IdentityService.Application.DTOs.AdminDtos;
using IdentityService.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace IdentityService.API.Controllers
{
    /// <summary>
    /// Handles administrator operations for managing user accounts.
    /// </summary>
    [Route("api/v1/admin")]
    [ApiController]
    [Authorize(Roles = "Admin", Policy = "VerifiedEmail")]
    public class AdminController(IUserService userService) : ControllerBase
    {
        /// <summary>
        /// Retrieves a paginated list of users.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of users per page.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <param name="status">User status : Active/Inactive/All .</param>
        /// <returns>A paginated collection of users.</returns>
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(UserStatus status,int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var result = await userService.GetUsersPaginatedWithCountAsync(status, pageNumber, pageSize, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The user's unique identifier.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>The requested user.</returns>
        [HttpGet("users/{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
        {
            var result = await userService.GetUserByIdAsync(id, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Changes the role assigned to a user.
        /// </summary>
        /// <param name="id">The user's unique identifier.</param>
        /// <param name="dto">The requested role change.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the role was updated successfully.</returns>
        [HttpPatch("users/{id:guid}/role")]
        public async Task<IActionResult> ChangeUserRole(Guid id, ChangeUserRoleDto dto, CancellationToken cancellationToken)
        {
            await userService.ChangeUserRoleAsync(id, dto.Role, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Activates a deactivated user account.
        /// </summary>
        /// <param name="id">The user's unique identifier.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the account was activated successfully.</returns>
        [HttpPatch("users/{id:guid}/activate")]
        public async Task<IActionResult> ActivateUser(Guid id, CancellationToken cancellationToken)
        {
            await userService.ActivateUserAsync(id, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Deactivates an active user account.
        /// </summary>
        /// <param name="id">The user's unique identifier.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the account was deactivated successfully.</returns>
        [HttpPatch("users/{id:guid}/deactivate")]
        public async Task<IActionResult> DeactivateUser(Guid id, CancellationToken cancellationToken)
        {
            await userService.DeactivateUserAsync(id, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Permanently deletes a user account.
        /// </summary>
        /// <param name="id">The user's unique identifier.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>No content if the user was deleted successfully.</returns>
        [HttpDelete("users/{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
        {
            await userService.DeleteUserAsync(id, cancellationToken);

            return NoContent();
        }
    }
}
