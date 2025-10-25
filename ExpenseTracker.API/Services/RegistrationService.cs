using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Mappings;

namespace ExpenseTracker.API.Services
{
    public class RegistrationService
        (
            IUserService userService,
            IJwtTokenService jwtTokenService
        )
        : IRegistrationService
    {
        public async Task<Result<RegisterResultDTO>> Register(RegisterDTO dto)
        {
            var result = await userService.AddUser(dto.ToCreateUserDto());
            if (!result.Success)
            {
                return Result<RegisterResultDTO>.BadRequestResult(result.ErrorMessage!);
            }

            var token = jwtTokenService.CreateAccessToken(result.Data!.AsJwtUserData());
            return Result<RegisterResultDTO>.OkResult(new RegisterResultDTO(token));
        }
    }
}
