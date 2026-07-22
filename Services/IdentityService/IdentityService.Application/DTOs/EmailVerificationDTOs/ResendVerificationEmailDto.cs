using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.DTOs.EmailVerificationDTOs
{
    public class ResendVerificationEmailDto
    {
        public string Email { get; set; } = default!;
    }
}
