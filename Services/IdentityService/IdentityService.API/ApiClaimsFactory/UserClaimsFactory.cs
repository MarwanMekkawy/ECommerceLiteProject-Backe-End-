using IdentityService.Application.Claims;
using IdentityService.Domain.Enums;
using System.Security.Claims;

namespace IdentityService.API.ApiClaimsFactory
{
    public static class UserClaimsFactory
    {
        public static UserClaims ExtractFrom(ClaimsPrincipal user)
        {
            return new UserClaims(
            UserId: Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!),
            UserName: user.FindFirstValue(ClaimTypes.Name)!,
            UserEmail: user.FindFirstValue(ClaimTypes.Email)!,
            UserRole: Enum.Parse<RoleType>(user.FindFirstValue(ClaimTypes.Role)!)
            );
        }
    }
}
