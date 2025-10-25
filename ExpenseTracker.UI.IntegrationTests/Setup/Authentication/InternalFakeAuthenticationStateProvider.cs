using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Authorization;
using ExpenseTracker.UI.Authentication;
using System.Security.Claims;

namespace ExpenseTracker.IntegrationTests.Setup.Authentication
{
    internal class InternalFakeAuthenticationStateProvider
        (
            ITokenExtractor tokenExtractor,
            TestAuthorizationContext testAuthorizationContext
        )
        : FakeAuthenticationStateProvider, IJwtAuthStateProvider
    {
        public void NotifyUserAuthentication(string token)
        {
            var identity = new ClaimsIdentity(tokenExtractor.GetUserClaims(token), AuthConstants.AUTHENTICATION_TYPE);
            var user = new ClaimsPrincipal(identity);

            var fullName = user.Claims?.FirstOrDefault(c => c.Type == AuthConstants.NAME_CLAIM_TYPE)?.Value
                ?? throw new InvalidOperationException($"Not found claim {AuthConstants.NAME_CLAIM_TYPE}");
            testAuthorizationContext.SetAuthorized(fullName);
            testAuthorizationContext.SetClaims([.. identity.Claims]);
            TriggerAuthenticationStateChanged(fullName, [], user.Claims, identity.AuthenticationType);
        }

        public void NotifyUserLogout()
        {
            testAuthorizationContext.SetNotAuthorized();
            TriggerUnauthenticationStateChanged();
        }

        public new Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var claimsIdentity = new ClaimsIdentity(testAuthorizationContext.Claims, AuthConstants.AUTHENTICATION_TYPE, AuthConstants.NAME_CLAIM_TYPE, null);
            var user = new ClaimsPrincipal(claimsIdentity);
            var authState = new AuthenticationState(user);
            return Task.FromResult(authState);
        }
    }
}
