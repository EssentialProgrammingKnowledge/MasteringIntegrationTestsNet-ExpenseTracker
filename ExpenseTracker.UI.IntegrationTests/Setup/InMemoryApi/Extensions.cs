using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using ExpenseTracker.IntegrationTests.Setup.DataStore;
using ExpenseTracker.UI.Authentication;
using RichardSzalay.MockHttp;

namespace ExpenseTracker.IntegrationTests.Setup.InMemoryApi
{
    public static partial class Extensions
    {
        public static IServiceCollection AddInMemoryApi(this IServiceCollection services)
        {
            return services.AddSingleton<IInMemoryDataStore, InMemoryDataStore>()
                           .AddScoped<MockHttpMessageHandler, InternalMockHttpMessageHandler>()
                           .AddScoped(sp =>
                           {
                               var store = sp.GetRequiredService<IInMemoryDataStore>();
                               var tokenStore = sp.GetRequiredService<ITokenStore>();
                               var mockHttpMessageHandler = sp.GetRequiredService<MockHttpMessageHandler>();
                               var testAuthorizationContext = sp.GetRequiredService<TestAuthorizationContext>();
                               return store.CreateInMemoryHttpClient(tokenStore, mockHttpMessageHandler, testAuthorizationContext);
                           });
        }

        public static HttpClient CreateInMemoryHttpClient(this IInMemoryDataStore store, ITokenStore tokenStore, MockHttpMessageHandler mockHttp, TestAuthorizationContext testAuthorizationContext)
        {
            mockHttp.SetupEndpoints(store, testAuthorizationContext);
            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri("http://backend");
            return client;
        }
    }
}
