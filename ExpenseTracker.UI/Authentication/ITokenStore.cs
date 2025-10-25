namespace ExpenseTracker.UI.Authentication
{
    public interface ITokenStore
    {
        Task<string?> GetTokenAsync();
        Task SetTokenAsync(string token);
        Task RemoveTokenAsync();
    }
}
