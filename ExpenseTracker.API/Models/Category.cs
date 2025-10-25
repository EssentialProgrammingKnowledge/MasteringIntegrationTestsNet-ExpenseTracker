namespace ExpenseTracker.API.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public List<Expense> Expenses { get; set; } = [];
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
