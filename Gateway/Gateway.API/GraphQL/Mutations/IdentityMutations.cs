using Gateway.API.Abstractions;
using Gateway.API.DTOs;
using HotChocolate.Authorization;

namespace Gateway.API.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class IdentityMutations
    {
        // Email
        [Authorize]
        public async Task<IdentityMutationPayload> ResendVerification(IIdentityServiceClient client, CancellationToken cancellationToken = default) 
        { 
            await client.ResendVerificationAsync(cancellationToken); 
            return new IdentityMutationPayload(true); 
        }
        [Authorize(Policy = "VerifiedEmail")]
        public async Task<IdentityMutationPayload> RequestEmailChange(IIdentityServiceClient client, ChangeEmailRequestDto request, CancellationToken cancellationToken = default)
        {
            await client.RequestEmailChangeAsync(request, cancellationToken);
            return new IdentityMutationPayload(true);
        }

        // Current user
        [Authorize(Policy = "VerifiedEmail")]
        public async Task<IdentityMutationPayload> UpdateCurrentUser(IIdentityServiceClient client, UpdateUserRequestDto request, CancellationToken cancellationToken = default) 
        { 
            await client.UpdateCurrentUserAsync(request, cancellationToken); 
            return new IdentityMutationPayload(true); 
        }
        [Authorize]
        public async Task<IdentityMutationPayload> DeactivateCurrentUser(IIdentityServiceClient client, CancellationToken cancellationToken = default) 
        { 
            await client.DeactivateCurrentUserAsync(cancellationToken); 
            return new IdentityMutationPayload(true); 
        }
        [Authorize(Policy = "VerifiedEmail")]
        public async Task<IdentityMutationPayload> ChangePassword(IIdentityServiceClient client, ChangePasswordRequestDto request, CancellationToken cancellationToken = default) 
        { 
            await client.ChangePasswordAsync(request, cancellationToken); 
            return new IdentityMutationPayload(true); 
        }

        // Admin       
        [Authorize(Roles = ["Admin"], Policy = "VerifiedEmail")]
        public async Task<IdentityMutationPayload> ChangeUserRole(IIdentityServiceClient client, Guid id, ChangeUserRoleRequestDto request, CancellationToken cancellationToken = default)
        { 
            await client.ChangeUserRoleAsync(id, request, cancellationToken); 
            return new IdentityMutationPayload(true); 
        }
        [Authorize(Roles = ["Admin"], Policy = "VerifiedEmail")]
        public async Task<IdentityMutationPayload> ActivateUser(IIdentityServiceClient client, Guid id, CancellationToken cancellationToken = default) 
        { 
            await client.ActivateUserAsync(id, cancellationToken); 
            return new IdentityMutationPayload(true); 
        }
        [Authorize(Roles = ["Admin"], Policy = "VerifiedEmail")]
        public async Task<IdentityMutationPayload> DeactivateUser(IIdentityServiceClient client, Guid id, CancellationToken cancellationToken = default) 
        { 
            await client.DeactivateUserAsync(id, cancellationToken); 
            return new IdentityMutationPayload(true); 
        }
        [Authorize(Roles = ["Admin"], Policy = "VerifiedEmail")]
        public async Task<IdentityMutationPayload> DeleteUser(IIdentityServiceClient client, Guid id, CancellationToken cancellationToken = default)
        {
            await client.DeleteUserAsync(id, cancellationToken);
            return new IdentityMutationPayload(true);
        }
    }
    public sealed record IdentityMutationPayload(bool Success);
}
