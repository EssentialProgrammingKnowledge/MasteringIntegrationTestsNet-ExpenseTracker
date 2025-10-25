using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using ExpenseTracker.UI;

namespace ExpenseTracker.IntegrationTests.Setup
{
    public class TestFixture
    {
        private BaseTestContext testContext = new ();
        private FakeNavigationManager navigation { get; set; }
        public string CurrentUrl => navigation.Uri;

        public BaseTestContext TestContext
        {
            get
            {
                if (testContext is null || testContext.IsDisposed)
                {
                    testContext = new();
                    return testContext;
                }

                return testContext;
            }
        }

        public IRenderedComponent<App> AppComponent { get; private set; }
        public IRenderedComponent<CascadingAuthenticationState> CascadingAuthenticationState { get; private set; }

        public TestFixture()
        {
            navigation = (TestContext.Services.GetRequiredService<NavigationManager>() as FakeNavigationManager)!;
            ArgumentNullException.ThrowIfNull(navigation);
            var authenticationStateProvider = TestContext.Services.GetRequiredService<AuthenticationStateProvider>();
            authenticationStateProvider.AuthenticationStateChanged += (_) => {
                AppComponent?.Render();
            };
            CascadingAuthenticationState = TestContext.RenderComponent<CascadingAuthenticationState>(ps =>
                ps.AddChildContent<App>());
            AppComponent = CascadingAuthenticationState.FindComponent<App>();
        }

        public void NavigateTo(string path, bool forceLoad = false)
        {
            navigation = (TestContext.Services.GetRequiredService<NavigationManager>() as FakeNavigationManager)!;
            ArgumentNullException.ThrowIfNull(navigation);
            navigation?.NavigateTo(path, forceLoad);

            if (!forceLoad)
            {
                AppComponent?.Render();
                return;
            }

            // Fixed routing problems, check module 7 lesson 3
            navigation?.NavigateTo("not-extists");
            AppComponent?.Render();
            navigation?.NavigateTo(path, forceLoad);
            AppComponent?.Render();
        }
    }
}
