using ExpenseTracker.API.DTO;

namespace ExpenseTracker.API.Services
{
    public interface IJwtTokenService
    {
        string CreateAccessToken(JwtUserDataDTO user);
    }
}
