using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entities
{
    public class ServiceClient : BaseEntity
    {
        public string ClientId { get; private set; } = null!;
        public string ClientSecretHash { get; private set; } = null!;
        public string ServiceName { get; private set; } = null!;
        public bool IsActive { get; private set; }

        private ServiceClient() { }

        public ServiceClient(string clientId, string clientSecretHash, string serviceName)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentException("Client ID is required.");

            if (string.IsNullOrWhiteSpace(clientSecretHash))
                throw new ArgumentException("Client secret hash is required.");

            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentException("Service name is required.");

            ClientId = clientId.Trim();
            ClientSecretHash = clientSecretHash;
            ServiceName = serviceName.Trim();
            IsActive = true;
        }

        public void Activate()
        {
            if (IsActive)
                throw new InvalidOperationException("Service client is already active.");
            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new InvalidOperationException("Service client is already inactive.");

            IsActive = false;
        }
    }
}
