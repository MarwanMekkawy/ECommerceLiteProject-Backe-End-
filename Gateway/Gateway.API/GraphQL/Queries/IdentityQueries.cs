using Gateway.API.Abstractions;
using Gateway.API.DTOs;

namespace Gateway.API.GraphQL.Queries
{
    [ExtendObjectType(typeof(Query))]
    public class IdentityQueries
    {
        // Current user
        public Task<UserDto> GetCurrentUser(IIdentityServiceClient client, CancellationToken cancellationToken = default)
            => client.GetCurrentUserAsync(cancellationToken);

        // Admin
        public Task<PagedResult<UserDto>> GetUsers(IIdentityServiceClient client, PaginationQueryDto query, CancellationToken cancellationToken = default)
            => client.GetUsersAsync(query, cancellationToken);

        public Task<UserDto> GetUserById(IIdentityServiceClient client, Guid id, CancellationToken cancellationToken = default)
            => client.GetUserByIdAsync(id, cancellationToken);
    }
}
