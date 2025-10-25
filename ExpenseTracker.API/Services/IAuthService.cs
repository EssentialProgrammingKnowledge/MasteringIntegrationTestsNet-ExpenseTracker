using ExpenseTracker.API.DTO;

namespace ExpenseTracker.API.Services
{
    public interface IAuthService
    {
        Task<Result<LoginResultDTO>> Login(LoginDTO dto);
    }
}
