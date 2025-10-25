using Microsoft.AspNetCore.Components.Authorization;

namespace ExpenseTracker.UI.Authentication
{
    public static class Extensions
    {
        public static IServiceCollection AddAuthentication(this IServiceCollection services)
        {
            services.AddAuthorizationCore();
            services.AddScoped<ITokenStore, TokenStore>();
            services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();
            services.AddScoped<IJwtAuthStateProvider, JwtAuthStateProvider>(sp => (JwtAuthStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
            services.AddScoped<IAuthStateService, AuthStateService>();
            services.AddScoped<JwtAuthorizationDelegatingHandler>();
            services.AddSingleton<ITokenExtractor, TokenExtractor>();
            services.AddSingleton<IUserStore, UserStore>();
            return services;
        }
    }
}
