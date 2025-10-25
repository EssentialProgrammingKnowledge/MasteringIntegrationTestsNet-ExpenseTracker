using ExpenseTracker.API.Cache;
using ExpenseTracker.API.Database;
using ExpenseTracker.API.Repositories;
using ExpenseTracker.API.Security;
using ExpenseTracker.API.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddInMemoryCache();
builder.Services.AddApiCors(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();
app.UseApiCors();
app.UseExceptionHandler();
app.UseJwtAuthenticationAndAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }
