using ExpenseTracker.UI.Models;

namespace ExpenseTracker.UI.Services
{
    public interface IAuthService
    {
        Task<Result<LoginResultDTO>> Login(LoginDTO dto);
        Task<Result> Validate();
    }
}
