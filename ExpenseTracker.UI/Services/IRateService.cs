using ExpenseTracker.UI.Models;

namespace ExpenseTracker.UI.Services
{
    public interface IRateService
    {
        Task<Result<List<RateDTO>>> GetAll();
    }
}
