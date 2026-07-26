using IdentityService.Application.DTOs.PwResetDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Abstractions
{
    public interface IPasswordResetService
    {
        Task<string> RequestPasswordResetAsync(ForgotPasswordDto dto, CancellationToken cancellationToken);
        Task ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken);
    }
}
