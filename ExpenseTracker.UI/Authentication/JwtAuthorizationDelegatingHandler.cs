using System.Net.Http.Headers;

namespace ExpenseTracker.UI.Authentication
{
    public class JwtAuthorizationDelegatingHandler : DelegatingHandler
    {
        private readonly ITokenStore _tokenStore;

        public JwtAuthorizationDelegatingHandler(ITokenStore tokenStore)
        {
            _tokenStore = tokenStore;
            InnerHandler = new HttpClientHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenStore.GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(AuthConstants.AUTHENTICATION_TYPE, token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
