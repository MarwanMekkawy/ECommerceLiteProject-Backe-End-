using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Abstractions
{
    public interface IAuthService
    {
        Task<(string? JwtToken, string? RefreshToken)> RegisterAsync(RegisterDto dto);
        Task<(string? JwtToken, string? RefreshToken)> LoginAsync(LoginDto dto);
        Task ChangePasswordAsync(int userId, string oldPassword, string newPassword);
        Task LogoutAsync(string refreshToken);
        Task<(string? JwtToken, string? RefreshToken)> RefreshSessionAsync(string refreshToken);
    }
}
