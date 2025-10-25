using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetById(int id);
        Task<bool> ExistsByEmail(string email);
        Task<User?> GetByEmail(string email);
        Task<User> Add(User user);
    }
}
