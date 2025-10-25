namespace ExpenseTracker.API.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public Currency Currency { get; set; } = Currency.PLN;
        public decimal Rate { get; set; } = decimal.One;
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public decimal GetTotalAmount()
        {
            return Amount * Rate;
        }
    }
}
