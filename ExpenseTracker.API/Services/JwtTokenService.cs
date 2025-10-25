using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExpenseTracker.API.Services
{
    internal sealed class JwtTokenService
        (
            IOptionsMonitor<JwtSettings> jwtSettingsOptions,
            ILogger<JwtTokenService> logger
        )
        : IJwtTokenService
    {
        public string CreateAccessToken(JwtUserDataDTO user)
        {
            var jwtSettings = jwtSettingsOptions.CurrentValue;
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.IssuerSigningKey)),
                SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new (JwtClaimConstants.SUB, user.Id.ToString()),
                new (JwtClaimConstants.USER_ID, user.UserId.ToString()),
                new (JwtClaimConstants.EMAIL, user.Email),
                new (JwtClaimConstants.GIVEN_NAME, user.FirstName),
                new (JwtClaimConstants.FAMILY_NAME, user.LastName),
                new (JwtClaimConstants.NAME, $"{user.FirstName} {user.LastName}")
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings.IssuerName,
                audience: jwtSettings.AudienceName,
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(jwtSettings.ExpireTimeSeconds),
                signingCredentials: signingCredentials);

            logger.LogTrace("Create token for user with id '{UserId}'", user.Id);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
