using IdentityService.Domain.Contracts;
using IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace IdentityService.Infrastructure.Repositories
{
    public class ServiceClientRepository(IdentityDbContext _context) : IServiceClientRepository
    {
        public async Task<ServiceClient?> GetByClientIdAsync(string clientId, CancellationToken cancellationToken = default)
        {
            return await _context.ServiceClients.FirstOrDefaultAsync(x => x.ClientId == clientId, cancellationToken);
        }
    }
}
