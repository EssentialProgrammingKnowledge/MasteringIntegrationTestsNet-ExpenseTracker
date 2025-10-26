using ExpenseTracker.API.Database;
using ExpenseTracker.API.IntegrationTests.Setup.Auth;
using ExpenseTracker.API.IntegrationTests.Setup.Database;
using ExpenseTracker.API.IntegrationTests.Setup.Logging;
using ExpenseTracker.API.IntegrationTests.TestServices;
using ExpenseTracker.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace ExpenseTracker.API.IntegrationTests.Setup.Hosting
{
    public class WebAppFactoryFixture : WebApplicationFactory<Program>
    {
        public HttpClient Client { get; }
        public TestUserAccessor UserAccessor { get; }
        public Action<string, LogLevel, string>? LogAction { get; set; }

        private readonly string _databaseName = $"expense-tracker-test-db-{Guid.NewGuid()}";
        private readonly Mock<INbpRatesService> _nbpRatesSevice = new(); 
        private readonly bool _dropDatabase = true;
        private bool _disposed;

        public WebAppFactoryFixture()
        {
            Client = CreateClient();
            UserAccessor = Services.GetRequiredService<TestUserAccessor>();
            var config = Services.GetRequiredService<IConfiguration>();
            _dropDatabase = config.GetValue<bool>("DropDatabase");
        }

        public void SetupNbpRateService(Action<Mock<INbpRatesService>> setup)
            => setup(_nbpRatesSevice);

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped((_) => new CategoryTestService(Client!));
                services.AddScoped((_) => new ExpenseTestService(Client!));
                services.AddSingleton<IDatabaseNameProvider, TestDatabaseNameProvider>((_) => new TestDatabaseNameProvider(_databaseName));
                services.AddSingleton<ITokenParser, TokenParser>();
                services.AddSingleton((_) => _nbpRatesSevice.Object);
                services.AddSingleton<TestUserAccessor>();
                services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
            });
            builder.ConfigureLogging(logging => logging.AddProvider(new DelegateLoggerProvider((category, level, message) =>
            {
                LogAction?.Invoke(category, level, message);
            })));
            builder.UseEnvironment("test");
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            LogAction = null;
            if (_dropDatabase)
            {
                using var scope = Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ExpenseContext>();
                dbContext.Database.EnsureDeleted();
            }

            base.Dispose(disposing);
        }
    }
}
