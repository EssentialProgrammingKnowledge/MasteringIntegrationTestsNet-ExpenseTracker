using Microsoft.EntityFrameworkCore;
using ExpenseTracker.API.Database;
using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Repositories
{
    internal sealed class UserRepository
        (
            ExpenseContext expenseContext
        )
        : IUserRepository
    {
        public async Task<User> Add(User user)
        {
            await expenseContext.Users.AddAsync(user);
            await expenseContext.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetById(int id)
        {
            return await expenseContext.Users
                                     .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<User?> GetByEmail(string email)
        {
            return await expenseContext.Users
                                     .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<bool> ExistsByEmail(string email)
        {
            return await expenseContext.Users.AnyAsync(u => u.Email == email);
        }
    }
}
