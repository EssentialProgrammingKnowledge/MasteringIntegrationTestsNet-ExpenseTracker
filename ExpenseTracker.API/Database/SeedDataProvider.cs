using ExpenseTracker.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Database
{
    public class SeedDataProvider
        (
            ExpenseContext expenseContext,
            IPasswordHasher<User> passwordHasher
        ) : ISeedDataProvider
    {
        public async Task SeedData(CancellationToken cancellationToken = default)
        {
            var users = await SeedUsers(cancellationToken);
            var result = await SeedCategories(users, cancellationToken);
            await SeedExpenses(result, cancellationToken);
        }

        private async Task<List<User>> SeedUsers(CancellationToken cancellationToken = default)
        {
            if (await expenseContext.Users.AnyAsync(cancellationToken))
            {
                return [];
            }

            var users = new List<User>
            {
                new() { Email = "stanislaw.wokulski@gmail.com", FirstName = "Stanisław", LastName = "Wokulski", Password = "Stachu123!Abc" },
                new() { Email = "alicja@krainaczarow.com", FirstName = "Alicja", LastName = "Czarodziejka", Password = "Czarodziejka123!" }
            };

            foreach (var user in users)
            {
                user.Password = passwordHasher.HashPassword(user, user.Password);
            }

            await expenseContext.Users.AddRangeAsync(users, cancellationToken);
            await expenseContext.SaveChangesAsync(cancellationToken);
            return users;
        }

        private async Task<List<CategorySeedEntry>> SeedCategories(List<User> users, CancellationToken cancellationToken = default)
        {
            if (await expenseContext.Categories.AnyAsync(cancellationToken))
            {
                return [];
            }

            var categories = new List<CategorySeedEntry>();
            foreach (var user in users)
            {
                var homeExpenseCategory = new Category
                {
                    Name = "Domowe wydatki",
                    Budget = 15000,
                    User = user
                };
                categories.Add(new CategorySeedEntry(CategoryType.HOME, homeExpenseCategory));

                var roadExpenseCategory = new Category
                {
                    Name = "Wydatki na drogę",
                    Budget = 1000,
                    User = user
                };
                categories.Add(new CategorySeedEntry(CategoryType.ROAD, roadExpenseCategory));

                var gameExpenseCategory = new Category
                {
                    Name = "Budżet na grę",
                    Budget = 100,
                    User = user
                };
                categories.Add(new CategorySeedEntry(CategoryType.GAME, gameExpenseCategory));
            }

            await expenseContext.Categories.AddRangeAsync(categories.Select(c => c.Category), cancellationToken);
            await expenseContext.SaveChangesAsync(cancellationToken);
            return categories;
        }

        private async Task SeedExpenses(List<CategorySeedEntry> categories, CancellationToken cancellationToken = default)
        {
            if (await expenseContext.Expenses.AnyAsync(cancellationToken))
            {
                return;
            }

            var expenses = new List<Expense>();
            foreach (var categoryEntry in categories)
            {
                switch (categoryEntry.CategoryType)
                {
                    case CategoryType.HOME:
                        expenses.AddRange(
                            new Expense
                            {
                                Description = "Laptop Dell",
                                Amount = 6500,
                                Category = categoryEntry.Category,
                                User = categoryEntry.Category.User
                            },
                            new Expense
                            {
                                Description = "Zmywarka Bosch",
                                Amount = 2500,
                                Category = categoryEntry.Category,
                                User = categoryEntry.Category.User
                            },
                            new Expense
                            {
                                Description = "Pralka",
                                Amount = 3000,
                                Category = categoryEntry.Category,
                                User = categoryEntry.Category.User
                            });
                        break;
                    case CategoryType.ROAD:
                        expenses.AddRange(
                            new Expense
                            {
                                Description = "Paliwo Orlen",
                                Amount = 500,
                                Category = categoryEntry.Category,
                                User = categoryEntry.Category.User
                            },
                            new Expense
                            {
                                Description = "Paliwo BP",
                                Amount = 250,
                                Category = categoryEntry.Category,
                                User = categoryEntry.Category.User
                            },
                            new Expense
                            {
                                Description = "Opłata za bramki",
                                Amount = 50,
                                Category = categoryEntry.Category,
                                User = categoryEntry.Category.User
                            });
                        break;
                    case CategoryType.GAME:
                        expenses.Add(
                            new Expense
                            {
                                Description = "Skórki",
                                Amount = 50,
                                Category = categoryEntry.Category,
                                User = categoryEntry.Category.User
                            });
                        break;
                }
            }

            await expenseContext.Expenses.AddRangeAsync(expenses, cancellationToken);
            await expenseContext.SaveChangesAsync(cancellationToken);
        }

        private enum CategoryType
        {
            HOME,
            ROAD,
            GAME
        }

        private record CategorySeedEntry(CategoryType CategoryType, Category Category);
    }
}
