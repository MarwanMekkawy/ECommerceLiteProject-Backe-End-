using AutoMapper;
using Domain.Exceptions;
using IdentityService.Application.Abstractions;
using IdentityService.Application.Abstractions.Authentication;
using IdentityService.Application.DTOs.UserDTOs;
using IdentityService.Domain.Contracts;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Services
{
    public class UserService(IUnitOfWork uow, IMapper mapper, IPasswordHasher hasher) : IUserService
    {
        #region//[helper methods]==================================================================================
        private async Task<User> GetUserOrThrowAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await uow.users.GetByIdAsync(userId, cancellationToken) ?? throw new NotFoundException("User not found");
        }

        private static bool IsStrongPassword(string password)
        {
            return password.Length >= 8 && password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) && password.Any(char.IsDigit);
        }

        private static void ValidatePassword(string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(password)) throw new BadRequestException("Password can't be empty.");

            if (password != confirmPassword) throw new BadRequestException("Password confirmation must match the password.");

            if (password.Length < 8) throw new BadRequestException("Password must be at least 8 characters.");

            if (!IsStrongPassword(password)) throw new BadRequestException("Password is too weak.");
        }
        #endregion//================================================================================================

        public async Task<UserDto> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(userId, cancellationToken);
            return mapper.Map<UserDto>(user);
        }

        public async Task UpdateProfileAsync(Guid userId, UpdateUserDto dto, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(userId, cancellationToken);
            mapper.Map(dto, user);
            await uow.SaveChangesAsync(cancellationToken);
        }

        public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(userId, cancellationToken);
            ValidatePassword(dto.NewPassword, dto.ConfirmNewPassword);
            if (!hasher.Verify(user.PasswordHash,dto.CurrentPassword)) throw new BadRequestException("wrong current password");
            user.PasswordHash = hasher.Hash(dto.NewPassword);
            await uow.SaveChangesAsync(cancellationToken);
        }

        public async Task DeactivateAccountAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(userId, cancellationToken);
            if (user.IsActive == false) throw new BadRequestException("user account already Deactivated");
            user.Deactivate();
            await uow.SaveChangesAsync(cancellationToken);
        }


        public async Task<IEnumerable<UserDto>> GetUsersAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var users = await uow.users.GetPagedAsync(page, pageSize, cancellationToken);
            return mapper.Map < IEnumerable < UserDto >> (users);
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(id, cancellationToken);
            return mapper.Map<UserDto>(user);
        }

        public async Task ActivateUserAsync(Guid id, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(id, cancellationToken);
            if (user.IsActive == true) throw new BadRequestException("user account already activated");
            user.Activate();
            await uow.SaveChangesAsync(cancellationToken);
        }

        public async Task DeactivateUserAsync(Guid id, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(id, cancellationToken);
            if (user.IsActive == false) throw new BadRequestException("user account already Deactivated");
            user.Deactivate();
            await uow.SaveChangesAsync(cancellationToken);
        }

        public async Task ChangeUserRoleAsync(Guid id, RoleType role, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(id, cancellationToken);
            user.ChangeRole(role);
            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
