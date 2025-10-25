using ExpenseTracker.UI.Models;

namespace ExpenseTracker.UI.Services
{
    public interface IRegisterService
    {
        Task<Result<RegisterResultDTO>> Register(RegisterDTO dto);
    }
}
