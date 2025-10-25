using ExpenseTracker.UI.Authentication;
using RichardSzalay.MockHttp;
using System.Net.Http.Headers;

namespace ExpenseTracker.IntegrationTests.Setup.InMemoryApi
{
    internal class InternalMockHttpMessageHandler
        (
            ITokenStore tokenStore
        )
        : MockHttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await tokenStore.GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(AuthConstants.AUTHENTICATION_TYPE, token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
