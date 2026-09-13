using IdentityService.Application.DTOs.AuthDTOs;

namespace IdentityService.Application.Abstractions.ClientsAbstractions
{
    public interface IServiceClientService
    {
        Task<AuthResponseDto> AuthinticateAsync(string clientId, string clientSecret, CancellationToken cancellationToken);
    }
}
