using ExpenseTracker.IntegrationTests.Setup.DataStore.Models;

namespace ExpenseTracker.IntegrationTests.Setup.DataStore
{
    public interface IInMemoryDataStore
    {
        public IDictionary<int, InMemoryCategory> Categories { get; }
        public IDictionary<int, InMemoryExpense> Expenses { get; }
        public IDictionary<int, InMemoryUser> Users { get; }
    }
}
