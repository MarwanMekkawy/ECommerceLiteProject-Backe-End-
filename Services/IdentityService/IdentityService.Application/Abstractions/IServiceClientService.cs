using IdentityService.Application.DTOs.AuthDTOs;
using IdentityService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Abstractions
{
    public interface IServiceClientService
    {
        Task<AuthResponseDto> AuthinticateAsync(string clientId, string clientSecret, CancellationToken cancellationToken);
    }
}
