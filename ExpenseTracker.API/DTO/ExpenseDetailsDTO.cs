using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.DTO
{
    public record ExpenseDetailsDTO(int Id, decimal Amount, string Description, Currency Currency, decimal Rate, CategoryDTO Category);
}
