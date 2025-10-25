using ExpenseTracker.UI.Models;

namespace ExpenseTracker.IntegrationTests.Setup.DataStore.Models
{
    public class InMemoryExpense
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public Currency Currency { get; set; } = Currency.PLN;
        public decimal Rate { get; set; } = decimal.One;
        public int CategoryId { get; set; }
        public int UserId { get; set; }

        public InMemoryExpense() { }

        public InMemoryExpense(
                int id,
                string description,
                decimal amount,
                Currency currency,
                decimal rate,
                int categoryId,
                int userId
            )
        {
            Id = id;
            Description = description;
            Amount = amount;
            Currency = currency;
            Rate = rate;
            CategoryId = categoryId;
            UserId = userId;
        }
    }
}
