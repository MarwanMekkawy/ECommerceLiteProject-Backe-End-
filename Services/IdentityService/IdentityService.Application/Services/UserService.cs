using AutoMapper;
using Domain.Exceptions;
using IdentityService.Application.Abstractions;
using IdentityService.Application.Abstractions.Authentication;
using IdentityService.Application.DTOs.UserDTOs;
using IdentityService.Domain.Contracts;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;


namespace IdentityService.Application.Services
{
    public class UserService(IUnitOfWork uow, IMapper mapper, IPasswordHasher hasher, IRefreshTokenService refreshTokenService) : IUserService
    {
        #region//[helper methods]==================================================================================

        private async Task<User> GetUserOrThrowAsync(bool admin, Guid userId, CancellationToken cancellationToken)
        {
            if (admin) return await uow.users.GetByIdIncludingInactiveAsync(userId, cancellationToken) ?? throw new NotFoundException("User not found");

            return await uow.users.GetByIdAsync(userId, cancellationToken) ?? throw new NotFoundException("User not found or inactive");
        }

        private bool IsStrongPassword(string password)
        {
            return password.Any(char.IsUpper) && password.Any(char.IsLower) && password.Any(char.IsDigit);
        }

        private void ValidatePassword(string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(password)) throw new BadRequestException("Password can't be empty.");

            if (password != confirmPassword) throw new BadRequestException("Password confirmation must match the password.");

            if (password.Length < 8) throw new BadRequestException("Password must be at least 8 characters.");

            if (!IsStrongPassword(password)) throw new BadRequestException("Password is too weak.");
        }

        private async Task OldPasswordReuseCheckAndCycle(User user, string newPassword, CancellationToken cancellationToken)
        {
            if (hasher.Verify(user.PasswordHash, newPassword))
                throw new ConflictException("New password must be different from the current password.");

            const int MaxPasswordHistory = 3;
            var usedBeforePasswordsHash = await uow.userPasswordHistories.GetAllByUserIdAsync(user.Id, MaxPasswordHistory, cancellationToken);

            foreach (var pw in usedBeforePasswordsHash)
            {
                if (hasher.Verify(pw.PasswordHash, newPassword))
                    throw new ConflictException("You cannot reuse one of your recent passwords");
            }

            if (usedBeforePasswordsHash.Count >= MaxPasswordHistory)
            {
                uow.userPasswordHistories.Delete(usedBeforePasswordsHash[MaxPasswordHistory - 1]);
            }

            var currentPasswordSaveHistory = new UserPasswordHistory() { UserId = user.Id, PasswordHash = user.PasswordHash };

            await uow.userPasswordHistories.AddAsync(currentPasswordSaveHistory, cancellationToken);
        }

        #endregion ================================================================================================

        public async Task<UserDto> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(admin: false, userId, cancellationToken);
            return mapper.Map<UserDto>(user);
        }

        public async Task UpdateProfileAsync(Guid userId, UpdateUserDto dto, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(admin: false, userId, cancellationToken);
            mapper.Map(dto, user);
            await uow.SaveChangesAsync(cancellationToken);
        }

        public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(admin: false, userId, cancellationToken);

            ValidatePassword(dto.NewPassword, dto.ConfirmNewPassword);

            if (!hasher.Verify(user.PasswordHash, dto.CurrentPassword)) 
                throw new BadRequestException("wrong current password");

            await OldPasswordReuseCheckAndCycle(user, dto.NewPassword, cancellationToken);

            user.ChangePassword(hasher.Hash(dto.NewPassword));

            await uow.SaveChangesAsync(cancellationToken);
        }

        public async Task DeactivateAccountAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(admin: false, userId, cancellationToken);
            if (user.IsActive == false) 
                throw new BadRequestException("user account already Deactivated");
            user.Deactivate();
            await refreshTokenService.RevokeAllUserRefreshTokensAsync(userId, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);
        }

        // admin
        public async Task<IEnumerable<UserDto>> GetUsersAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var users = await uow.users.GetPagedAsync(page, pageSize, cancellationToken);
            return mapper.Map < IEnumerable < UserDto >> (users);
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(admin: true, id, cancellationToken);
            return mapper.Map<UserDto>(user);
        }

        public async Task ActivateUserAsync(Guid id, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(admin: true, id, cancellationToken);          
            user.Activate();
            await uow.SaveChangesAsync(cancellationToken);
        }

        public async Task DeactivateUserAsync(Guid id, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(admin: true, id, cancellationToken);            
            user.Deactivate();
            await refreshTokenService.RevokeAllUserRefreshTokensAsync(id, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);
        }

        public async Task ChangeUserRoleAsync(Guid id, RoleType role, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(admin: true, id, cancellationToken);
            user.ChangeRole(role);
            await uow.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(admin: true, id, cancellationToken);
            uow.users.Delete(user);
            await refreshTokenService.RevokeAllUserRefreshTokensAsync(id, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
