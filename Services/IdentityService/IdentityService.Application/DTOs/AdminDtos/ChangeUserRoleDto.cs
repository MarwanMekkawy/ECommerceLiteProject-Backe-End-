using IdentityService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.DTOs.AdminDtos
{
    public record ChangeUserRoleDto(RoleType Role);
}
