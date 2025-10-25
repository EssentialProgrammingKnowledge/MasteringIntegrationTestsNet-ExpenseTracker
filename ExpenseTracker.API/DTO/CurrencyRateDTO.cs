using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.DTO
{
    public record CurrencyRateDTO(Currency Currency, decimal Rate, DateOnly RateDate);
}
