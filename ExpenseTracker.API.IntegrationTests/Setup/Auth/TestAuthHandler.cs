using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ExpenseTracker.API.Security;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ExpenseTracker.API.IntegrationTests.Setup.Auth
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly TestUserAccessor _userAccessor;

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            TestUserAccessor userAccessor)
            : base(options, logger, encoder)
        {
            _userAccessor = userAccessor;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var user = _userAccessor.CurrentUser;
            if (user == null)
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var identity = new ClaimsIdentity(user.Claims, "Test", JwtClaimConstants.NAME, null);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
