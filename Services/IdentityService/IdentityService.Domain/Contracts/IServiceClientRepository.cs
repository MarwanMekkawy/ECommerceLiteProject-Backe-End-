using IdentityService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Contracts
{
    public interface IServiceClientRepository
    {
        Task<ServiceClient?> GetByClientIdAsync(string clientId, CancellationToken cancellationToken);
    }
}
