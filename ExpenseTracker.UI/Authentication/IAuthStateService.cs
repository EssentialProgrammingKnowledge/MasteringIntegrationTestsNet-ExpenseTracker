using ExpenseTracker.UI.Models;

namespace ExpenseTracker.UI.Authentication
{
    public interface IAuthStateService
    {
        UserProfile? GetUserProfile();
        Task LoginAsync(string token);
        Task LogoutAsync();
    }
}
