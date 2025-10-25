using ExpenseTracker.API.Models;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.API.Services
{
    public static class Extensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddSingleton<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRegistrationService, RegistrationService>();
            services.AddHttpContextAccessor();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddScoped<INbpRatesService, NbpRatesService>();
            services.AddScoped<ICurrencyRateService, CurrencyRateService>();
            services.AddHttpClient("NbpClient", client =>
            {
                client.BaseAddress = new Uri("https://api.nbp.pl/");
            });
            return services;
        }
    }
}
