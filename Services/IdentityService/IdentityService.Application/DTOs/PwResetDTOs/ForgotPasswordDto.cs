using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.DTOs.PwResetDTOs
{
    public class ForgotPasswordDto
    {
        public string Email { get; set; } = default!;
    }
}
