using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Npgsql;

namespace ExpenseTracker.API.Database
{
    public static class Extensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.SetupDatabase(configuration);
            services.AddTransient<ISeedDataProvider, SeedDataProvider>();
            services.AddHostedService<DbInitializer>();
            return services;
        }

        private static IServiceCollection SetupDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseSettings>(configuration.GetSection(nameof(DatabaseSettings)));
            services.AddDbContext<ExpenseContext>((sp, options) =>
            {
                var dbOptions = sp.GetRequiredService<IOptionsMonitor<DatabaseSettings>>();
                dbOptions.CurrentValue.Validate();
                var dbNameProvider = sp.GetService<IDatabaseNameProvider>();

                switch (dbOptions.CurrentValue.Provider)
                {
                    case DatabaseProvider.PostgreSQL:
                        {
                            if (dbNameProvider is not null)
                            {
                                var csb = new NpgsqlConnectionStringBuilder(dbOptions.CurrentValue.ConnectionString)
                                {
                                    Database = dbNameProvider.DatabaseName
                                };

                                dbOptions.CurrentValue.ConnectionString = csb.ConnectionString;
                            }

                            options.UseNpgsql(dbOptions.CurrentValue.ConnectionString);
                            break;
                        }
                    case DatabaseProvider.InMemory:
                        {
                            options.UseInMemoryDatabase(dbNameProvider?.DatabaseName ?? "ExpenseDb");
                            break;
                        }
                    default:
                        throw new InvalidOperationException("Not supported database");
                }
            });
            return services;
        }
    }
}
