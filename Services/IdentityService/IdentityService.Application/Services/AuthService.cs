using Domain.Exceptions;
using IdentityService.Application.Abstractions;
using IdentityService.Application.Abstractions.Authentication;
using IdentityService.Application.DTOs.AuthDTOs;
using IdentityService.Domain.Contracts;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Exceptions;
using System.Net.Mail;


namespace IdentityService.Application.Services
{
    public class AuthService(IUnitOfWork uow, IPasswordHasher hasher, IJwtTokenService jwt, IRefreshTokenService refreshTokenService) : IAuthService
    {
        #region //[helper methods]========================================================      
        private bool IsStrongPassword(string password)
        {
            return password.Any(char.IsUpper) && password.Any(char.IsLower) && password.Any(char.IsDigit);
        }
        private string NormalizeEmail(string email)
        {
            return email?.Trim().ToLowerInvariant() ?? string.Empty;
        }
        private bool IsValidEmail(string email)
        {
            return MailAddress.TryCreate(email, out _);
        }
        private void ValidatePassword(string Password, string ConfirmPassword) 
        {
            if (string.IsNullOrWhiteSpace(Password)) throw new BadRequestException("Password Cant be empty");
            if (Password != ConfirmPassword) throw new BadRequestException("New password confirmation must match the password");
            if (Password.Length < 8) throw new BadRequestException("New password must be at least 8 characters");
            if (!IsStrongPassword(Password)) throw new BadRequestException("Password is too weak");
        }
        #endregion//========================================================================

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto, CancellationToken cancellationToken)
        {
            // Check if email format is valid
            var normalizedEmail = NormalizeEmail(dto.Email);
            if (!IsValidEmail(normalizedEmail))
                throw new BadRequestException("Invalid email format");

            // Check if user already exists 
            var existingEmail = await uow.users.GetByEmailAsync(normalizedEmail, cancellationToken);
            if (existingEmail != null) 
                throw new ConflictException("Email is already in use");

            //password validation
            ValidatePassword(dto.Password, dto.ConfirmPassword);

            var newUser = new User
            {
                Email = normalizedEmail,
                PasswordHash = hasher.Hash(dto.Password),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber
            };
            await uow.users.AddAsync(newUser, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);

            return new RegisterResponseDto { userId = newUser.Id };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken)
        {
            var normalizedEmail = NormalizeEmail(dto.Email);

            var existingUser = await uow.users.GetByEmailAsync(normalizedEmail, cancellationToken);
            var existingPwHash = existingUser?.PasswordHash ?? "AQAAAAIAAYagAAAAEAAAAAAAAAAAAAAAAAAAAABIN8F2glsG2w0ThRc6b//V2SgXfV/+/2ZFaUf66RukGA==";

            var isPasswordValid = hasher.Verify(existingPwHash, dto.Password);

            var isValidLogin = existingUser != null && isPasswordValid;
            if (!isValidLogin) throw new UnauthorizedException($"Wrong password or Email");

            var newJwtToken = jwt.GenerateAccessToken(existingUser!);
            var newRefreshToken = await refreshTokenService.CreateAndStoreRefreshTokenAsync(existingUser!.Id, cancellationToken);

            await uow.SaveChangesAsync(cancellationToken);
            return new AuthResponseDto { AccessToken = newJwtToken , RefreshToken = newRefreshToken.PlaintextToken };
        }

        public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken)
        {
            await refreshTokenService.RevokeRefreshTokenAsync(refreshToken, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);
        }

        public async Task<AuthResponseDto> RefreshSessionAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var rotatedToken = await refreshTokenService.RotateRefreshTokenAsync(refreshToken, cancellationToken);
            if (rotatedToken == null) throw new InvalidTokenException("Refresh token reuse detected. All sessions revoked.");
            var (stored, plaintextToken) = rotatedToken.Value;
            var user = await uow.users.GetByIdAsync(stored.UserId, cancellationToken) ?? throw new NotFoundException("User not found");

            var newJwt = jwt.GenerateAccessToken(user);
            await uow.SaveChangesAsync(cancellationToken);
            return new AuthResponseDto { AccessToken = newJwt, RefreshToken = plaintextToken };
        }
    }
}
