using IdentityService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Claims
{
    public record UserClaims(Guid UserId, string UserName, string UserEmail, RoleType UserRole);
}
