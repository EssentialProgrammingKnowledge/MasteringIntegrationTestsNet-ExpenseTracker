using Blazored.LocalStorage;
using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using ExpenseTracker.IntegrationTests.Setup.Authentication;
using ExpenseTracker.IntegrationTests.Setup.DataStore;
using ExpenseTracker.IntegrationTests.Setup.InMemoryApi;
using ExpenseTracker.IntegrationTests.Setup.Services;
using ExpenseTracker.UI.Authentication;
using ExpenseTracker.UI.Languages;
using ExpenseTracker.UI.Services;

namespace ExpenseTracker.IntegrationTests.Setup
{
    public class BaseTestContext : TestContext, IDisposable
    {
        public TestAuthorizationContext AuthContext { get; }
        public bool IsDisposed { get; private set; }

        public BaseTestContext()
        {
            Services.AddMudServices();
            Services.AddServices();
            Services.AddTranslations();
            Services.AddBlazoredLocalStorage();
            Services.AddAuthentication();
            JSInterop.Mode = JSRuntimeMode.Loose;
            AuthContext = this.AddTestAuthorization();
            Services.AddSingleton(AuthContext);
            Services.AddSingleton<AuthenticationStateProvider, InternalFakeAuthenticationStateProvider>();
            Services.AddSingleton<IJwtAuthStateProvider>(sp => (InternalFakeAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
            Services.AddInMemoryApi();
            Services.DecorateExistingServices();

            var storage = new Dictionary<string, string>();
            Services.AddScoped<ILocalStorageService, InternalLocalStorageService>(_ =>  new InternalLocalStorageService(storage));
            Services.AddSingleton<IUserStore, InternalUserStore>();
        }


        public new void Dispose()
        {
            if (!IsDisposed)
            {
                base.Dispose();
                IsDisposed = true;
            }
        }
    }
}
