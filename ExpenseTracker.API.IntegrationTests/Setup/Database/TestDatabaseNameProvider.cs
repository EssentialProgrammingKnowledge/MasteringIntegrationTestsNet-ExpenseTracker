using ExpenseTracker.API.Database;

namespace ExpenseTracker.API.IntegrationTests.Setup.Database
{
    internal class TestDatabaseNameProvider : IDatabaseNameProvider
    {
        public TestDatabaseNameProvider(string databaseName)
        {
            DatabaseName = databaseName;
        }

        public string DatabaseName { get; }
    }
}
