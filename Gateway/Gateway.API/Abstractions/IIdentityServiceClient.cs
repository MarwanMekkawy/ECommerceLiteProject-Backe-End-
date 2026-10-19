using Gateway.API.DTOs;

namespace Gateway.API.Abstractions
{
    public interface IIdentityServiceClient
    {
        // Email
        Task ResendVerificationAsync(CancellationToken cancellationToken = default);
        Task RequestEmailChangeAsync(ChangeEmailRequestDto request, CancellationToken cancellationToken = default);

        // Current user
        Task<UserDto> GetCurrentUserAsync(CancellationToken cancellationToken = default);
        Task UpdateCurrentUserAsync(UpdateUserRequestDto request, CancellationToken cancellationToken = default);
        Task DeactivateCurrentUserAsync(CancellationToken cancellationToken = default);
        Task ChangePasswordAsync(ChangePasswordRequestDto request, CancellationToken cancellationToken = default);

        // Admin
        Task<PagedResult<UserDto>> GetUsersAsync(PaginationQueryDto query, CancellationToken cancellationToken = default);
        Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
        Task ChangeUserRoleAsync(Guid id, ChangeUserRoleRequestDto request, CancellationToken cancellationToken = default);
        Task ActivateUserAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeactivateUserAsync(Guid id, CancellationToken cancellationToken = default);
    }
}