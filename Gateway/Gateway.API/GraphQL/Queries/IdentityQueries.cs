using Gateway.API.Abstractions;
using Gateway.API.DTOs;
using HotChocolate.Authorization;

namespace Gateway.API.GraphQL.Queries
{
    [ExtendObjectType(typeof(Query))]
    public class IdentityQueries
    {
        // Current user
        [Authorize]
        public Task<UserDto> GetCurrentUser(IIdentityServiceClient client, CancellationToken cancellationToken = default)
            => client.GetCurrentUserAsync(cancellationToken);

        // Admin
        [Authorize(Roles = ["Admin"], Policy = "VerifiedEmail")]
        public Task<PagedResult<UserDto>> GetUsers(IIdentityServiceClient client, PaginationQueryDto query, CancellationToken cancellationToken = default)
            => client.GetUsersAsync(query, cancellationToken);
        [Authorize(Roles = ["Admin"], Policy = "VerifiedEmail")]
        public Task<UserDto> GetUserById(IIdentityServiceClient client, Guid id, CancellationToken cancellationToken = default)
            => client.GetUserByIdAsync(id, cancellationToken);
    }
}
