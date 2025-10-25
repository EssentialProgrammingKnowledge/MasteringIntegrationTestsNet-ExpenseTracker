namespace ExpenseTracker.IntegrationTests.Setup.DataStore.Models
{
    public class InMemoryCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public int UserId { get; set; }

        public InMemoryCategory() { }

        public InMemoryCategory(
                int id,
                string name,
                decimal budget,
                int userId
            )
        {
            Id = id;
            Name = name;
            Budget = budget;
            UserId = userId;
        }
    }
}
