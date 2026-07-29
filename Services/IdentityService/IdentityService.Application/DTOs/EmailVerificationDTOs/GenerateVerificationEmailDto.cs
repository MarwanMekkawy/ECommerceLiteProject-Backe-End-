using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.DTOs.EmailVerificationDTOs
{
    public class GenerateVerificationEmailDto
    {
        public string Email { get; set; } = default!;
        public string Token { get; set; } = default!;
    }
}
