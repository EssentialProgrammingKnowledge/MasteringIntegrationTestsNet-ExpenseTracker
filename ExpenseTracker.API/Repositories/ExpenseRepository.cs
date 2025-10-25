using ExpenseTracker.API.Database;
using ExpenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Repositories
{
    internal sealed class ExpenseRepository
        (
            ExpenseContext context
        )
        : IExpenseRepository
    {
        public async Task<Expense> Add(Expense expense)
        {
            await context.Expenses.AddAsync(expense);
            await context.SaveChangesAsync();
            return expense;
        }

        public async Task<bool> DeleteByIdAndUserId(int id, int userId)
        {
            var expense = await context.Expenses
                                       .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
            if (expense is null)
            {
                return false;
            }

            context.Expenses.Remove(expense);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Expense>> GetAllByUserId(int userId)
        {
            return await context.Expenses.Where(e => e.UserId == userId).ToListAsync();
        }

        public async Task<Expense?> GetByIdAndUserId(int id, int userId)
        {
            return await context.Expenses
                                .Include(e => e.Category)
                                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        }

        public async Task<decimal> GetTotalExpensesAmount(int categoryId)
        {
            var expenses = await context.Expenses.Where(e => e.CategoryId == categoryId)
                                                 .Select(e => new Expense() { Rate = e.Rate, Amount = e.Amount })
                                                 .ToListAsync();

            return expenses.Sum(e => e.GetTotalAmount());
        }

        public async Task<Expense> Update(Expense expense)
        {
            context.Expenses.Update(expense);
            await context.SaveChangesAsync();
            return expense;
        }
    }
}
