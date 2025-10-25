using Blazored.LocalStorage;

namespace ExpenseTracker.UI.Authentication
{
    public class TokenStore
        (
            ILocalStorageService localStorageService
        ) : ITokenStore
    {
        public async Task<string?> GetTokenAsync()
        {
            return await localStorageService.GetItemAsStringAsync(AuthConstants.TOKEN_STORE_NAME);
        }

        public async Task RemoveTokenAsync()
        {
            await localStorageService.RemoveItemAsync(AuthConstants.TOKEN_STORE_NAME);
        }

        public async Task SetTokenAsync(string token)
        {
            await localStorageService.SetItemAsStringAsync(AuthConstants.TOKEN_STORE_NAME, token);
        }
    }
}
