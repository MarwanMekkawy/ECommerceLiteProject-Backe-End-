using IdentityService.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entities
{
    public class EmailChangeToken : BaseEntity
    {
        public EmailChangeToken()
        {
            ExpiresAt = DateTime.UtcNow.AddHours(24);
        }

        public string TokenHash { get; set; } = default!;

        public Guid UserId { get; set; }

        public string NewEmail { get; set; } = default!;

        public DateTime ExpiresAt { get; set; } 

        public DateTime? ConfirmedAt { get; set; }

        public User User { get; set; } = default!;

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsConfirmed => ConfirmedAt != null;
        public bool IsActive => !IsExpired && !IsConfirmed;

        public void Confirm()
        {
            if (!IsActive)
                throw new InvalidTokenException("Invalid or expired email change token.");

            ConfirmedAt = DateTime.UtcNow;
        }
    }
}
