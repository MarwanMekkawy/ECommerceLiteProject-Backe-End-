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
    /// Handles admin user management operations.
    /// </summary>
    [Route("api/V1/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController(IUserService userService) : ControllerBase
    {
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var result = await userService.GetUsersAsync(pageNumber, pageSize, cancellationToken);

            return Ok(result);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
        {
            var result = await userService.GetUserByIdAsync(id, cancellationToken);

            return Ok(result);
        }

        [HttpPatch("users/{id}/role")]
        public async Task<IActionResult> ChangeUserRole(Guid id, ChangeUserRoleDto dto, CancellationToken cancellationToken)
        {
            await userService.ChangeUserRoleAsync(id, dto.Role, cancellationToken);

            return NoContent();
        }

        [HttpPatch("users/{id}/activate")]
        public async Task<IActionResult> ActivateUser(Guid id, CancellationToken cancellationToken)
        {
            await userService.ActivateUserAsync(id, cancellationToken);

            return NoContent();
        }

        [HttpPatch("users/{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(Guid id, CancellationToken cancellationToken)
        {
            await userService.DeactivateUserAsync(id, cancellationToken);   

            return NoContent();
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
        {
            await userService.DeleteUserAsync(id, cancellationToken);   

            return NoContent();
        }
    }
}
