using IdentityService.Application.DTOs.AuthDTOs;

namespace IdentityService.Application.Abstractions.ClientsAbstractions
{
    public interface ISelfServiceClientService
    {
        Task<string> SelfAuthinticateAsync(CancellationToken cancellationToken);
    }
}
