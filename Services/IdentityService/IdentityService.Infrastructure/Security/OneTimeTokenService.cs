using IdentityService.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;


namespace IdentityService.Infrastructure.Security
{
    public class OneTimeTokenService : IOneTimeTokenService
    {
        public string GenerateToken()
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(32);
            return WebEncoders.Base64UrlEncode(bytes);                     //stringfy [Base64Url] shorter string
        }

        public string HashToken(string token)
        {
            byte[] tokenBytes = Encoding.UTF8.GetBytes(token);             

            byte[] hash = SHA256.HashData(tokenBytes);

            return WebEncoders.Base64UrlEncode(hash);
        }
    }
}
