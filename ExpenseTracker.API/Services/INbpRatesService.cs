using ExpenseTracker.API.DTO;

namespace ExpenseTracker.API.Services
{
    public interface INbpRatesService
    {
        Task<IEnumerable<NbpRateDTO>> GetRatesAsync();
    }
}
