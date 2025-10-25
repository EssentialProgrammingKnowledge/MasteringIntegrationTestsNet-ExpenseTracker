using ExpenseTracker.API.DTO;

namespace ExpenseTracker.API.Services
{
    public interface IExpenseService
    {
        Task<Result<ExpenseDetailsDTO>> AddExpense(ExpenseDTO expenseDto, int userId);
        Task<Result<ExpenseDetailsDTO>> UpdateExpense(ExpenseDTO expenseDto, int userId);
        Task<Result> DeleteExpense(int id, int userId);
        Task<Result<ExpenseDetailsDTO>> GetExpenseById(int id, int userId);
        Task<IEnumerable<ExpenseDTO>> GetAllExpenses(int userId);
    }
}
