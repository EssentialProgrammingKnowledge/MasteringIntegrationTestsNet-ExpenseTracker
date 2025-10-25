using ExpenseTracker.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ExpenseTracker.UI.Authentication
{
    public class JwtAuthStateProvider
        (
            ITokenStore tokenStore,
            ITokenExtractor tokenExtractor,
            IAuthService authService,
            IUserStore userStore
        )
        : AuthenticationStateProvider, IJwtAuthStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var accessToken = await tokenStore.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return Anonymous();
            }

            var validateResult = await authService.Validate();
            if (!validateResult.Valid)
            {
                await tokenStore.RemoveTokenAsync();
                return Anonymous();
            }

            var userClaims = tokenExtractor.GetUserClaims(accessToken);
            var identity = new ClaimsIdentity(userClaims, AuthConstants.AUTHENTICATION_TYPE, AuthConstants.NAME_CLAIM_TYPE, null);
            var user = new ClaimsPrincipal(identity);
            userStore.SetUserProfile(userClaims);

            return new AuthenticationState(user);
        }

        public void NotifyUserAuthentication(string token)
        {
            var userClaims = tokenExtractor.GetUserClaims(token);
            var identity = new ClaimsIdentity(userClaims, AuthConstants.AUTHENTICATION_TYPE);
            var user = new ClaimsPrincipal(identity);
            userStore.SetUserProfile(userClaims);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void NotifyUserLogout()
        {
            NotifyAuthenticationStateChanged(Task.FromResult(Anonymous()));
        }

        private AuthenticationState Anonymous()
        {
            userStore.ClearUserProfile();
            return new(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }
}
