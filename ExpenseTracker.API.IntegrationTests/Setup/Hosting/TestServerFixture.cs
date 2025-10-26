using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ExpenseTracker.API.Database;
using ExpenseTracker.API.IntegrationTests.Setup.Auth;
using ExpenseTracker.API.IntegrationTests.Setup.Database;
using ExpenseTracker.API.IntegrationTests.Setup.Logging;
using ExpenseTracker.API.Repositories;
using ExpenseTracker.API.Security;
using ExpenseTracker.API.Services;
using ExpenseTracker.API.IntegrationTests.TestServices;

namespace ExpenseTracker.API.IntegrationTests.Setup.Hosting
{
    public class TestServerFixture : IDisposable
    {
        public HttpClient Client { get; }
        public IServiceProvider Services { get; }
        public TestUserAccessor UserAccessor { get; }
        public Action<string, LogLevel, string>? LogAction { get; set; }

        private readonly TestServer _server;
        private readonly string _databaseName = $"expense-tracker-test-db-{Guid.NewGuid()}";
        private readonly Mock<INbpRatesService> _nbpRatesService = new();
        private readonly bool _dropDatabase = true;

        public TestServerFixture()
        {
            var builder = new WebHostBuilder()
                 .UseTestServer()
                 .UseEnvironment("test")
                 .ConfigureAppConfiguration((context, config) =>
                 {
                     config.Sources.Clear();
                     config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
                     config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json",
                         optional: true, reloadOnChange: false);
                     config.AddEnvironmentVariables();
                 })
                 .ConfigureServices((webHost, services) =>
                 {
                     services.AddScoped((_) => new CategoryTestService(Client!));
                     services.AddScoped((_) => new ExpenseTestService(Client!));
                     services.AddControllers()
                             .AddApplicationPart(typeof(Program).Assembly);
                     services.AddDatabase(webHost.Configuration);
                     services.AddRepositories();
                     services.AddServices();
                     services.AddProblemDetails();
                     services.AddApiCors(webHost.Configuration);
                     services.AddJwtAuthentication(webHost.Configuration);

                     services.AddSingleton((_) => _nbpRatesService.Object);

                     services.AddSingleton<IDatabaseNameProvider, TestDatabaseNameProvider>((_) => new TestDatabaseNameProvider(_databaseName));
                     services.AddSingleton<ITokenParser, TokenParser>();
                     services.AddSingleton<TestUserAccessor>();
                     services.AddAuthentication("Test")
                             .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                 })
                 .ConfigureLogging(logging => logging.AddProvider(new DelegateLoggerProvider((category, level, message) =>
                 {
                     LogAction?.Invoke(category, level, message);
                 })))
                 .Configure(app =>
                 {
                     app.UseRouting();
                     app.UseApiCors();
                     app.UseExceptionHandler();
                     app.UseJwtAuthenticationAndAuthorization();
                     app.UseEndpoints(endpoints =>
                     {
                         endpoints.MapControllers();
                     });
                 });

            _server = new TestServer(builder);
            Client = _server.CreateClient();
            Services = _server.Host.Services;
            UserAccessor = Services.GetRequiredService<TestUserAccessor>();
            var config = Services.GetRequiredService<IConfiguration>();
            _dropDatabase = config.GetValue<bool>("DropDatabase");
        }

        public void SetupNbpRateService(Action<Mock<INbpRatesService>> setup)
            => setup(_nbpRatesService);

        public void Dispose()
        {
            LogAction = null;
            if (_dropDatabase)
            {
                using var dbContext = _server.Services.GetRequiredService<ExpenseContext>();
                dbContext.Database.EnsureDeleted();
            }

            Client?.Dispose();
            _server.Dispose();
        }
    }
}
