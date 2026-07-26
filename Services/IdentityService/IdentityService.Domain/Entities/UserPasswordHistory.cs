using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entities
{
    public class UserPasswordHistory : BaseEntity
    {
        public Guid UserId { get; set; }
        public string PasswordHash { get; set; } = default!;
        public User User { get; set; } = default!;
    }
}
