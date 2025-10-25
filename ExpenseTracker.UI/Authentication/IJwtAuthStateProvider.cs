using Microsoft.AspNetCore.Components.Authorization;

namespace ExpenseTracker.UI.Authentication
{
    public interface IJwtAuthStateProvider
    {
        Task<AuthenticationState> GetAuthenticationStateAsync();

        void NotifyUserAuthentication(string token);

        void NotifyUserLogout();
    }
}
