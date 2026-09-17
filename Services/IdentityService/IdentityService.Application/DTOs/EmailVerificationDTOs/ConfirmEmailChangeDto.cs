namespace IdentityService.Application.DTOs.EmailVerificationDTOs
{
    public class ConfirmEmailChangeDto
    {
        public Guid UserId { get; set; }
        public string? OldEmail { get; set; }
        public string? FirstName { get; set; }
        public string? NewEmail { get; set; }
    }
}
