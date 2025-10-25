using Bunit.TestDoubles;
using ExpenseTracker.IntegrationTests.Setup.DataStore;
using ExpenseTracker.UI.Models;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ExpenseTracker.UI.IntegrationTests.Setup.InMemoryApi.EndpointsDefintions
{
    internal static class RatesEndpoints
    {
        private readonly static JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static void AddRatesEndpoints(this MockHttpMessageHandler mockHttp, IInMemoryDataStore store, TestAuthorizationContext testAuthorizationContext)
        {
            mockHttp.When(HttpMethod.Get, "/api/rates")
                .Respond(message =>
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(
                                                JsonSerializer.Serialize<IEnumerable<InMemoryCurrency>>([
                                                    new InMemoryCurrency(Currency.USD, 3.75M, DateOnly.FromDateTime(DateTime.UtcNow)),
                                                    new InMemoryCurrency(Currency.CHF, 4.55M, DateOnly.FromDateTime(DateTime.UtcNow)),
                                                    new InMemoryCurrency(Currency.EUR, 4.25M, DateOnly.FromDateTime(DateTime.UtcNow))
                                                ], JsonSerializerOptions),
                                                Encoding.UTF8, "application/json")
                    };
                });
        }

        private record InMemoryCurrency(Currency Currency, decimal Rate, DateOnly RateDate);

    }
}
