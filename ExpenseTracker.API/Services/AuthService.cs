using Microsoft.AspNetCore.Identity;
using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Mappings;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Repositories;
using ExpenseTracker.API.Validations;

namespace ExpenseTracker.API.Services
{
    internal sealed class AuthService
        (
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IJwtTokenService jwtTokenService,
            ILogger<AuthService> logger
        )
        : IAuthService
    {
        public async Task<Result<LoginResultDTO>> Login(LoginDTO dto)
        {
            var user = await userRepository.GetByEmail(dto.Email);
            if (user is null)
            {
                logger.LogInformation("User with email '{Email}' was not found", dto.Email);
                return Result<LoginResultDTO>.UnauthorizeResult(UserErrorMessages.InvalidCredentials());
            }

            if (passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password) == PasswordVerificationResult.Failed)
            {
                logger.LogWarning("User with email '{Email}' passed invalid password", dto.Email);
                return Result<LoginResultDTO>.UnauthorizeResult(UserErrorMessages.InvalidCredentials());
            }

            return Result<LoginResultDTO>.OkResult(new LoginResultDTO(jwtTokenService.CreateAccessToken(user.AsJwtUserData())));
        }
    }
}
