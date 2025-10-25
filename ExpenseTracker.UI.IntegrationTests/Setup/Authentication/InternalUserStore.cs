using Bunit.TestDoubles;
using ExpenseTracker.UI.Authentication;
using ExpenseTracker.UI.Models;
using System.Security.Claims;

namespace ExpenseTracker.IntegrationTests.Setup.Authentication
{
    internal class InternalUserStore
        (
            TestAuthorizationContext testAuthorizationContext
        ) : UserStore, IUserStore
    {
        public new void ClearUserProfile()
        {
            base.ClearUserProfile();
        }

        public new UserProfile? GetUserProfile()
        {
            SetUserProfile(testAuthorizationContext.Claims);
            return base.GetUserProfile();
        }

        public new void SetUserProfile(IEnumerable<Claim> claims)
        {
            if (claims is null)
            {
                ClearUserProfile();
                return;
            }

            base.SetUserProfile(claims);
        }
    }
}
