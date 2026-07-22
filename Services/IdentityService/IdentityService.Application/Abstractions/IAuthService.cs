using IdentityService.Application.DTOs.AuthDTOs;
using IdentityService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Abstractions
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto, CancellationToken cancellationToken);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken);
        Task LogoutAsync(string refreshToken, CancellationToken cancellationToken);
        Task<AuthResponseDto> RefreshSessionAsync(string refreshToken, CancellationToken cancellationToken);
    }
}
