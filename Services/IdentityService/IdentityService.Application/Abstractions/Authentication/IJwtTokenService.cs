using IdentityService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Abstractions.Authentication
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(User user);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
