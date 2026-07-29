using IdentityService.Application.DTOs.EmailVerificationDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Abstractions
{
    public interface IEmailVerificationTokenService
    {       
        Task<GenerateVerificationEmailDto> GenerateVerificationTokenAsync(Guid userId, CancellationToken cancellationToken);
        Task<GenerateVerificationEmailDto> ResendVerificationEmailAsync(Guid userId, CancellationToken cancellationToken);
        Task ConfirmEmailAsync(string token, CancellationToken cancellationToken);

        Task<string> GenerateEmailChangeTokenAsync(Guid userId, ChangeEmailRequestDto dto, CancellationToken cancellationToken);
        Task ConfirmEmailChangeAsync(string token, CancellationToken cancellationToken);
    }
}
