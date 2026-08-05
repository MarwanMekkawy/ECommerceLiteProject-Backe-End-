using Domain.Exceptions;
using IdentityService.Application.Abstractions;
using IdentityService.Application.Abstractions.Authentication;
using IdentityService.Application.DTOs.AuthDTOs;
using IdentityService.Domain.Contracts;


namespace IdentityService.Application.Services
{
    public class ServiceClientService(IUnitOfWork uow, IOneTimeTokenService oTTokenService,IJwtTokenService jwt) : IServiceClientService
    {
        public async Task<AuthResponseDto> AuthinticateAsync(string clientId, string clientSecret, CancellationToken cancellationToken)
        {
            var client = await uow.serviceClients.GetByClientIdAsync(clientId, cancellationToken);

            if (client is null)
                throw new BadRequestException("The Service doesnt exist.");

            if (!client.IsActive)
                throw new ForbiddenException("Service inactive or down.");

            var hashedSecret = oTTokenService.HashToken(clientSecret);

            if (hashedSecret != client.ClientSecretHash)
                throw new UnauthorizedException("Invalid service Credentials");

            return new AuthResponseDto { AccessToken = jwt.GenerateAccessTokenForClient(client) };
        }
    }
}
