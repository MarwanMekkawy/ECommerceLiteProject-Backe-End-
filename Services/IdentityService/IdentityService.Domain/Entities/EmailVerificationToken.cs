using IdentityService.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entities
{
    public class EmailVerificationToken : BaseEntity
    {
        public EmailVerificationToken()
        {
            ExpiresAt = DateTime.UtcNow.AddHours(24);
        }

        public string TokenHash { get; set; } = default!;
        public Guid UserId { get; set; }

        public DateTime ExpiresAt { get; set; }
        public DateTime? VerifiedAt { get; set; }

        public User User { get; set; } = default!;

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsVerified => VerifiedAt != null;
        public bool IsActive => !IsExpired && !IsVerified;

        public void MarkAsVerified()
        {
            if (!IsActive) throw new InvalidTokenException("the token is expired or used before");
            VerifiedAt = DateTime.UtcNow;
        }
    }
}
