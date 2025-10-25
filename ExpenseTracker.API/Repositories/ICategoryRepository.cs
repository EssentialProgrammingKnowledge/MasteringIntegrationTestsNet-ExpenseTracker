using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAndUserId(int id, int userId);
        Task<Category> Add(Category category);
        Task<Category> Update(Category category);
        Task<bool> Delete(Category category);
        Task<IEnumerable<Category>> GetAllByUserId(int userId);
        Task<bool> ContainExpenses(int id, int userId);
        Task<decimal> GetCategoriesTotalExpenses(int id, int userId);
    }
}
