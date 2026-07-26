using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.DTOs.EmailVerificationDTOs
{
    public class ChangeEmailRequestDto
    {
        public string NewEmail { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
