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
        Task<string> GenerateVerificationTokenAsync(Guid userId, CancellationToken cancellationToken);
        Task<string> ResendVerificationEmailAsync(Guid userId, CancellationToken cancellationToken);
        Task ConfirmEmailAsync(string token, CancellationToken cancellationToken);
    }
}
