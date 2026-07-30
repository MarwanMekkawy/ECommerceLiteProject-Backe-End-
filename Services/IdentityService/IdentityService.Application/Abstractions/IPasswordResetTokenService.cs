using IdentityService.Application.DTOs.PwResetDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Abstractions
{
    public interface IPasswordResetTokenService
    {
        Task<GeneratePasswordResetDto> RequestPasswordResetAsync(ForgotPasswordDto dto, CancellationToken cancellationToken);
        Task ResetPasswordAndLogOutAllDevicesAsync(string token, ResetPasswordDto dto, CancellationToken cancellationToken);
    }
}
