using AutoMapper;
using Domain.Exceptions;
using IdentityService.Application.Abstractions;
using IdentityService.Application.Abstractions.Authentication;
using IdentityService.Application.DTOs.UserDTOs;
using IdentityService.Domain.Contracts;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using System.Text.RegularExpressions;


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
            return password.Any(char.IsUpper) && password.Any(char.IsLower) && password.Any(char.IsDigit) && password.Any(c => char.IsPunctuation(c) || char.IsSymbol(c)); ;
        }
        private void ValidatePassword(string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(password)) throw new BadRequestException("Password can't be empty.");

            if (password != confirmPassword) throw new BadRequestException("Password confirmation must match the password.");

            if (password.Length < 8) throw new BadRequestException("Password must be at least 8 characters.");

            if (!IsStrongPassword(password)) 
                throw new BadRequestException("Password is too weak. It must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
        }
        private static void ValidatePhoneNumber(string? phoneNumber)
        {
            if (!string.IsNullOrWhiteSpace(phoneNumber) &&
                !Regex.IsMatch(phoneNumber, @"^\+?[1-9]\d{7,14}$"))
            {
                throw new BadRequestException("Invalid phone number format.");
            }
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

            ValidatePhoneNumber(dto.PhoneNumber);

            mapper.Map(dto, user);

            await uow.SaveChangesAsync(cancellationToken);
        }

        public async Task ChangePasswordAndLogOutAllDevicesAsync(Guid userId, ChangePasswordDto dto, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(admin: false, userId, cancellationToken);

            ValidatePassword(dto.NewPassword, dto.ConfirmNewPassword);

            if (!hasher.Verify(user.PasswordHash, dto.CurrentPassword)) 
                throw new BadRequestException("wrong current password");

            await OldPasswordReuseCheckAndCycle(user, dto.NewPassword, cancellationToken);

            user.ChangePassword(hasher.Hash(dto.NewPassword));
            await refreshTokenService.RevokeAllUserRefreshTokensAsync(userId, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);
        }

        public async Task DeactivateAccountAndLogOutAllDevicesAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(admin: false, userId, cancellationToken);
            if (user.IsActive == false) 
                throw new BadRequestException("user account already Deactivated");
            user.Deactivate();
            await refreshTokenService.RevokeAllUserRefreshTokensAsync(userId, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);
        }

        // admin ======================================================================================
        public async Task<PagedResult<UserDto>> GetUsersPaginatedWithCountAsync(UserStatus status, int page, int pageSize, CancellationToken cancellationToken)
        {
            IReadOnlyList<User> users;
            int totalCount;

            switch (status)
            {
                case UserStatus.Active:

                    users = await uow.users.GetPagedActiveAsync(page, pageSize, cancellationToken);
                    totalCount = await uow.users.GetActiveCountAsync(cancellationToken);

                    break;

                case UserStatus.Inactive:
                    users = await uow.users.GetPagedInactiveAsync(page, pageSize, cancellationToken);
                    var total = await uow.users.GetTotalCountAsync(cancellationToken);
                    var active = await uow.users.GetActiveCountAsync(cancellationToken);
                    totalCount = total - active;

                    break;

                default:
                    users = await uow.users.GetPagedAsync(page, pageSize, cancellationToken);
                    totalCount = await uow.users.GetTotalCountAsync(cancellationToken);

                    break;

            }

            return new PagedResult<UserDto>
            {
                Items = mapper.Map<IReadOnlyList<UserDto>>(users),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
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
