using ExpenseTracker.IntegrationTests.Setup.DataStore.Models;
using ExpenseTracker.UI.Models;

namespace ExpenseTracker.IntegrationTests.Setup.DataStore
{
    internal sealed class InMemoryDataStore : IInMemoryDataStore
    {
        private readonly Dictionary<int, InMemoryCategory> _categories = new()
        {
            { 1, new InMemoryCategory(1, "Sprzęt komputerowy", 20000M, 1) },
            { 2, new InMemoryCategory(2, "Sprzęt komputerowy", 20000M, 2) },
            { 3, new InMemoryCategory(3, "Szkolenie C#", 2000M, 1) },
            { 4, new InMemoryCategory(4, "Szkolenie C#", 2000M, 2) }
        };

        private readonly Dictionary<int, InMemoryExpense> _expenses = new()
        {
            { 1, new InMemoryExpense(1, "Laptop HP", 5000, Currency.CHF, 4.5M, 1, 1) },
            { 2, new InMemoryExpense(2, "Laptop Dell", 4500, Currency.CHF, 4.5M, 2, 2) },
            { 3, new InMemoryExpense(3, "Słuchawki Pioneer", 500, Currency.USD, 3.75M, 1, 1) },
            { 4, new InMemoryExpense(4, "Słuchawki AKG", 450, Currency.USD, 3.75M, 2, 2) },
            { 5, new InMemoryExpense(5, "Klawiatura", 50, Currency.PLN, decimal.One, 1, 1) },
            { 6, new InMemoryExpense(6, "Klawiatura", 50, Currency.PLN, decimal.One, 2, 2) },
            { 7, new InMemoryExpense(7, "Kurs programowania testów", 100M, Currency.PLN, decimal.One, 3, 1) },
            { 8, new InMemoryExpense(8, "Kurs programowania testów", 100M, Currency.PLN, decimal.One, 4, 2) },
            { 9, new InMemoryExpense(9, "Kurs programowania C#", 250M, Currency.PLN, decimal.One, 3, 1) },
            { 10, new InMemoryExpense(10, "Kurs programowania C#", 250M, Currency.PLN, decimal.One, 4, 2) }
        };

        private readonly Dictionary<int, InMemoryUser> _users = new()
        {
            { 1, new InMemoryUser(1, Guid.NewGuid(), "stanislaw.wokulski@gmail.com", "Stanisław", "Wokulski", "Stanisław Wokulski", "Stachu123!Abc" ) },
            { 2, new InMemoryUser(2, Guid.NewGuid(), "alicja@krainaczarow.com", "Alicja", "Czarodziejka", "Alicja Czarodziejka", "Czarodziejka123!" ) },
        };

        public IDictionary<int, InMemoryCategory> Categories => _categories;
        public IDictionary<int, InMemoryExpense> Expenses => _expenses;
        public IDictionary<int, InMemoryUser> Users => _users;
    }
}
