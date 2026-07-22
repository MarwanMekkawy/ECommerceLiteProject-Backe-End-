using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.DTOs.AuthDTOs
{
    public class RegisterResponseDto
    {
        public Guid userId { get; set; } = default!;
    }
}
