using IdentityService.Application.DTOs.EmailVerificationDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Abstractions
{
    public interface IEmailVerificationService
    {
        Task ConfirmEmailAsync(string token, CancellationToken cancellationToken);
        Task ResendVerificationEmailAsync(ResendVerificationEmailDto dto, CancellationToken cancellationToken);
    }
}
