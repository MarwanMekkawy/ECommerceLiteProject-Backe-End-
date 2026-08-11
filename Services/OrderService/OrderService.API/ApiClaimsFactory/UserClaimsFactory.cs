using OrderService.Application.Claims;
using System.Security.Claims;

namespace OrderService.API.ApiClaimsFactory
{
    public static class UserClaimsFactory
    {
        public static UserClaims ExtractFrom(ClaimsPrincipal user)
        {
            return new UserClaims(
            UserId: Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!),
            UserName: user.FindFirstValue(ClaimTypes.Name)!,
            UserEmail: user.FindFirstValue(ClaimTypes.Email)!,
            UserRole: (user.FindFirstValue(ClaimTypes.Role)!)
            );
        }
    }
}
