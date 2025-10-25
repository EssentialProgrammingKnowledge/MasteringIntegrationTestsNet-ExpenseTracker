using ExpenseTracker.UI.Models;

namespace ExpenseTracker.UI.Authentication
{
    public class AuthStateService
        (
            ITokenStore tokenStore,
            IJwtAuthStateProvider authStateProvider,
            IUserStore userStore
        ) : IAuthStateService
    {
        public UserProfile? GetUserProfile()
        {
            return userStore.GetUserProfile();
        }

        public async Task LoginAsync(string token)
        {
            await tokenStore.SetTokenAsync(token);
            authStateProvider.NotifyUserAuthentication(token);
        }

        public async Task LogoutAsync()
        {
            await tokenStore.RemoveTokenAsync();
            authStateProvider.NotifyUserLogout();
        }
    }
}
