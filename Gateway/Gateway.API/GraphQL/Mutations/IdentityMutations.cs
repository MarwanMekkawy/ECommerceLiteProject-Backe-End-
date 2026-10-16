using Gateway.API.Abstractions;
using Gateway.API.DTOs;

namespace Gateway.API.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class IdentityMutations
    {
        // Authentication
        public async Task<IdentityMutationPayload> Register(IIdentityServiceClient client, RegisterRequestDto request, CancellationToken cancellationToken = default)
        {
            await client.RegisterAsync(request, cancellationToken);
            return new IdentityMutationPayload(true);
        }

        public Task<JwtResponsetDto> Login(IIdentityServiceClient client, LoginRequestDto request, CancellationToken cancellationToken = default)
            => client.LoginAsync(request, cancellationToken);

        public async Task<IdentityMutationPayload> Logout(IIdentityServiceClient client, CancellationToken cancellationToken = default) 
        { 
            await client.LogoutAsync(cancellationToken); return new IdentityMutationPayload(true); 
        }

        public Task<JwtResponsetDto> RefreshToken(IIdentityServiceClient client, CancellationToken cancellationToken = default)
            => client.RefreshTokenAsync(cancellationToken);

        // Email
        public async Task<IdentityMutationPayload> ResendVerification(IIdentityServiceClient client, CancellationToken cancellationToken = default) 
        { 
            await client.ResendVerificationAsync(cancellationToken); 
            return new IdentityMutationPayload(true); 
        } 

        // Current user
        public async Task<IdentityMutationPayload> UpdateCurrentUser(IIdentityServiceClient client, UpdateUserRequestDto request, CancellationToken cancellationToken = default) 
        { 
            await client.UpdateCurrentUserAsync(request, cancellationToken); 
            return new IdentityMutationPayload(true); 
        } 
        public async Task<IdentityMutationPayload> DeactivateCurrentUser(IIdentityServiceClient client, CancellationToken cancellationToken = default) 
        { 
            await client.DeactivateCurrentUserAsync(cancellationToken); 
            return new IdentityMutationPayload(true); 
        } 
        public async Task<IdentityMutationPayload> ChangePassword(IIdentityServiceClient client, ChangePasswordRequestDto request, CancellationToken cancellationToken = default) 
        { 
            await client.ChangePasswordAsync(request, cancellationToken); 
            return new IdentityMutationPayload(true); 
        }

        // Admin
        public async Task<IdentityMutationPayload> DeleteUser(IIdentityServiceClient client, Guid id, CancellationToken cancellationToken = default) 
        { 
            await client.DeleteUserAsync(id, cancellationToken); 
            return new IdentityMutationPayload(true); 
        }
        public async Task<IdentityMutationPayload> ChangeUserRole(IIdentityServiceClient client, Guid id, ChangeUserRoleRequestDto request, CancellationToken cancellationToken = default)
        { 
            await client.ChangeUserRoleAsync(id, request, cancellationToken); 
            return new IdentityMutationPayload(true); 
        }
        public async Task<IdentityMutationPayload> ActivateUser(IIdentityServiceClient client, Guid id, CancellationToken cancellationToken = default) 
        { 
            await client.ActivateUserAsync(id, cancellationToken); 
            return new IdentityMutationPayload(true); 
        }
        public async Task<IdentityMutationPayload> DeactivateUser(IIdentityServiceClient client, Guid id, CancellationToken cancellationToken = default) 
        { 
            await client.DeactivateUserAsync(id, cancellationToken); 
            return new IdentityMutationPayload(true); 
        }
    }
    public sealed record IdentityMutationPayload(bool Success);
}
