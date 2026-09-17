using IdentityService.Application.Abstractions.Authentication;
using IdentityService.Application.Abstractions.ClientsAbstractions;
using IdentityService.Domain.Contracts;

namespace IdentityService.Application.Services
{
    public class SelfServiceClientService(IUnitOfWork uow, IJwtTokenService jwt) : ISelfServiceClientService
    {
        public async Task<string> SelfAuthinticateAsync(CancellationToken cancellationToken)
        {
            var self = await uow.serviceClients.GetByClientIdAsync("Identity-Service", cancellationToken) 
                ?? throw new InvalidOperationException("Identity service has no ServiceClient record for itself.");

            return jwt.GenerateAccessTokenForClient(self);
        }
    }
}