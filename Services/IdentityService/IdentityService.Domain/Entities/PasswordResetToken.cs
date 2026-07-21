using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entities
{
    public class PasswordResetToken : BaseEntity
    {
        public string TokenHash { get; set; } = default!;
        public Guid UserId { get; set; }

        public DateTime ExpiresAt { get; set; }
        public DateTime? UsedAt { get; set; }

        public User User { get; set; } = default!;

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsUsed => UsedAt != null;
        public bool IsActive => !IsExpired && !IsUsed;

        public void MarkAsUsed()
        {
            if (!IsActive) throw new InvalidTokenException();
            UsedAt = DateTime.UtcNow;
        }
    }
}
