using ExpenseTracker.API.IntegrationTests.Setup.Auth;
using ExpenseTracker.API.IntegrationTests.Setup.Hosting;
using Xunit.Abstractions;

namespace ExpenseTracker.API.IntegrationTests.Setup.BaseTests.CollectionFixtures
{
    public abstract class BaseTest
    {
        protected readonly HttpClient Client;
        protected readonly TestUserAccessor UserAccessor;
        protected readonly ITestOutputHelper TestOutputHelper;

        public BaseTest(WebAppFactoryFixture factoryFixture, ITestOutputHelper testOutputHelper)
        {
            Client = factoryFixture.Client;
            UserAccessor = factoryFixture.UserAccessor;
            TestOutputHelper = testOutputHelper;
            factoryFixture.LogAction = (category, level, message) =>
            {
                if (level <= Microsoft.Extensions.Logging.LogLevel.Debug)
                {
                    return;
                }

                testOutputHelper.WriteLine($"[{level} {category}: {message}]");
            };
        }
    }
}
