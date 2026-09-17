namespace IdentityService.Application.DTOs.PwResetDTOs
{
    public class GeneratePasswordResetDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public int ExpirationInMinutes { get; set; }
        public string Token { get; set; } = default!;
    }
}
