using IdentityService.Application.DTOs.UserDTOs;
using IdentityService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Abstractions
{
    public interface IUserService
    {
        Task<UserDto> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken);
        Task UpdateProfileAsync(Guid userId, UpdateUserDto dto, CancellationToken cancellationToken);
        Task ChangePasswordAndLogOutAllDevicesAsync(Guid userId, ChangePasswordDto dto, CancellationToken cancellationToken);
        Task DeactivateAccountAndLogOutAllDevicesAsync(Guid userId, CancellationToken cancellationToken);

        // Admin
        Task<PagedResult<UserDto>> GetUsersPaginatedWithCountAsync(UserStatus status, int page, int pageSize, CancellationToken cancellationToken);
        Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);
        Task ActivateUserAsync(Guid id, CancellationToken cancellationToken);
        Task DeactivateUserAsync(Guid id, CancellationToken cancellationToken);
        Task ChangeUserRoleAsync(Guid id, RoleType role, CancellationToken cancellationToken);
        Task DeleteUserAsync(Guid id, CancellationToken cancellationToken);
    }
}
