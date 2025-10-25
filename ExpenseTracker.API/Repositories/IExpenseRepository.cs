using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Repositories
{
    public interface IExpenseRepository
    {
        Task<Expense?> GetByIdAndUserId(int id, int userId);
        Task<Expense> Add(Expense expense);
        Task<Expense> Update(Expense expense);
        Task<bool> DeleteByIdAndUserId(int id, int userId);
        Task<IEnumerable<Expense>> GetAllByUserId(int userId);
        Task<decimal> GetTotalExpensesAmount(int categoryId);
    }
}
