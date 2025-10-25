namespace ExpenseTracker.API.Database
{
    public class DatabaseSettings
    {
        public DatabaseProvider Provider { get; set; }
        public string? ConnectionString { get; set; }

        public void Validate()
        {
            if (Provider == DatabaseProvider.InMemory)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(ConnectionString))
            {
                return;
            }

            throw new InvalidOperationException(
                $"'{nameof(ConnectionString)}' cannot be null or empty when Provider is '{Provider}'." +
                $"Example: \"Host=localhost;Database=ExpenseTrackerTest;Username=postgres;Password=postgres\"");
        }
    }

    public enum DatabaseProvider
    {
        InMemory, PostgreSQL
    }
}
