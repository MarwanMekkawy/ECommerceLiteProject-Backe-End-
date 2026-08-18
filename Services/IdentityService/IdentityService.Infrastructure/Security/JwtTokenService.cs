using IdentityService.Application.Abstractions.Authentication;
using IdentityService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace IdentityService.Infrastructure.Security
{
    public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
    {
        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("emailVerified", user.IsEmailConfirmed ? "true" : "false")
            };

            var secret = configuration["Jwt:Secret"];
            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("JWT secret is missing from configuration.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(configuration["Jwt:ExpiryInMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // just incase manual validation
        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],

                    ValidateLifetime = true
                }, out _);

                return principal;
            }
            catch
            {
                return null;
            }
        }

        public string GenerateAccessTokenForClient(ServiceClient client)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, client.ClientId),
                new Claim(ClaimTypes.Name, client.ServiceName),
                new Claim("token_type", "service")
            };


            var privateKey = configuration["JwtForServiceClient:PrivateKey"];
            if (string.IsNullOrWhiteSpace(privateKey))
                throw new InvalidOperationException("JWT service client private key is missing from configuration.");

            using var rsa = RSA.Create();

            try
            {
                rsa.ImportFromPem(privateKey.Replace("\\n", "\n"));
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException("JWT service client private key is invalid or has an invalid PEM format.", ex);
            }
            catch (CryptographicException ex)
            {
                throw new InvalidOperationException("JWT service client private key could not be imported.", ex);
            }

            var key = new RsaSecurityKey(rsa);
            var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],                               
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),                
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
