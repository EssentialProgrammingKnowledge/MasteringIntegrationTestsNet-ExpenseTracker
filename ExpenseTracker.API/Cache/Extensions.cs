using Microsoft.Extensions.Caching.Memory;

namespace ExpenseTracker.API.Cache
{
    public static class Extensions
    {
        public static IServiceCollection AddInMemoryCache(this IServiceCollection services)
        {
            return services.AddSingleton<IMemoryCache>((_) => new MemoryCache(new MemoryCacheOptions()));
        }
    }
}
