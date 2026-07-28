using Domain.Exceptions;
using IdentityService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string? PhoneNumber { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsActive { get; set; } = true;
        public RoleType Role { get; set; } = RoleType.Buyer;

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
        public ICollection<EmailVerificationToken> EmailVerificationTokens { get; set; } = new List<EmailVerificationToken>();
        public ICollection<EmailChangeToken> EmailChangeTokens { get; set; }  = new List<EmailChangeToken>();
        public ICollection<UserPasswordHistory> UserPasswordHistory { get; set; } = new List<UserPasswordHistory>();

        public void Activate()
        {
            if (IsActive == true) throw new BadRequestException("user account already activated");
            IsActive = true;
        }
        public void Deactivate()
        {
            if (IsActive == false) throw new BadRequestException("user account already Deactivated");
            IsActive = false;
        }
        public void ConfirmEmail()
        {
            IsEmailConfirmed = true;
        }
        public void ChangeRole(RoleType newRole)
        {
            if(Role == newRole) throw new BadRequestException($"the user account is already {Role}");
            Role = newRole;
        }
        public void ChangeEmail(string newEmail)
        {
            Email = newEmail;
        }
        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
        }
    }
}
