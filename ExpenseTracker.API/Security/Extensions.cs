using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ExpenseTracker.API.Security
{
    public static class Extensions
    {
        private const string CORS_POLICY = "ExpenseTrackerApiPolicy";
        public const string AUTHENTICATION_SCHEMA_NAME = "Bearer";

        public static IServiceCollection AddApiCors(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddCors(c =>
                c.AddPolicy(CORS_POLICY, policy =>
                    policy.AllowAnyHeader()
                          .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE")
                          .WithOrigins(configuration.GetValue<string>("Frontend") ?? throw new InvalidOperationException("Frontend url was not provided"))
            ));
        }

        public static IApplicationBuilder UseApiCors(this IApplicationBuilder app)
        {
            return app.UseCors(CORS_POLICY);
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
            services.AddAuthentication(AUTHENTICATION_SCHEMA_NAME)
                    .AddJwtBearer(AUTHENTICATION_SCHEMA_NAME, options =>
                    {
                        options.TokenValidationParameters = new()
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidIssuer = configuration.GetValue<string>("JwtSettings:IssuerName") ?? throw new InvalidOperationException("Section JwtSettings and property IssuerName was not provided"),
                            ValidAudience = configuration.GetValue<string>("JwtSettings:AudienceName") ?? throw new InvalidOperationException("Section JwtSettings and property AudienceName was not provided"),
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(configuration.GetValue<string>("JwtSettings:IssuerSigningKey") ?? throw new InvalidOperationException("Section JwtSettings and property IssuerSigningKey was not provided"))),
                            NameClaimType = JwtClaimConstants.SUB
                        };
                        options.MapInboundClaims = false;
                    });
            services.AddAuthorization();
            return services;
        }

        public static IApplicationBuilder UseJwtAuthenticationAndAuthorization(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}
