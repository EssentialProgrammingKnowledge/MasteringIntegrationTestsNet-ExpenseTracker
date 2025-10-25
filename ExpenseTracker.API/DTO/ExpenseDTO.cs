using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.DTO
{
    public record ExpenseDTO(int Id, decimal Amount, int CategoryId, string Description, Currency Currency, decimal Rate);
}
