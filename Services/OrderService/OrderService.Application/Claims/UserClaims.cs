namespace OrderService.Application.Claims
{
    public record UserClaims(Guid UserId, string UserName, string UserEmail, string UserRole);
}
