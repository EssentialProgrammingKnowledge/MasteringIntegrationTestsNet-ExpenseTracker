using ExpenseTracker.API.DTO;

namespace ExpenseTracker.API.Services
{
    public interface IRegistrationService
    {
        Task<Result<RegisterResultDTO>> Register(RegisterDTO dto);
    }
}
