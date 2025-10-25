namespace ExpenseTracker.API.Database
{
    public interface IDatabaseNameProvider
    {
        string DatabaseName { get; }
    }
}
