using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Abstractions.Authentication
{
    public interface IOneTimeTokenService
    {
        string GenerateToken();
        string HashToken(string token);
    }
}
