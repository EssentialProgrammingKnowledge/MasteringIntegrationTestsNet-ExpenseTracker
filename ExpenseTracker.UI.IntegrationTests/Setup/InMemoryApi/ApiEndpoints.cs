using Bunit.TestDoubles;
using ExpenseTracker.IntegrationTests.Setup.DataStore;
using ExpenseTracker.IntegrationTests.Setup.InMemoryApi.EndpointsDefintions;
using ExpenseTracker.UI.Authentication;
using ExpenseTracker.UI.IntegrationTests.Setup.InMemoryApi.EndpointsDefintions;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;

namespace ExpenseTracker.IntegrationTests.Setup.InMemoryApi
{
    public static class ApiEndpoints
    {
        private readonly static JsonSerializerOptions JsonSerializerOptions = new ()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static void SetupEndpoints(this MockHttpMessageHandler mockHttp, IInMemoryDataStore store, TestAuthorizationContext testAuthorizationContext)
        {
            mockHttp.AddCategoriesEndpoints(store, testAuthorizationContext);
            mockHttp.AddExpensesEndpoints(store, testAuthorizationContext);
            mockHttp.AddLoginEndpoints(store, testAuthorizationContext);
            mockHttp.AddRegisterEndpoints(store, testAuthorizationContext);
            mockHttp.AddUsersEndpoints(store, testAuthorizationContext);
            mockHttp.AddRatesEndpoints(store, testAuthorizationContext);
            mockHttp.Fallback.Respond(HttpStatusCode.NotFound);
        }

        internal static HttpResponseMessage? RequireAuthorization(this HttpRequestMessage message, TestAuthorizationContext testAuthorizationContext)
        {
            if (!testAuthorizationContext.IsAuthenticated)
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            return null;
        }

        internal static async Task<HttpResponseMessage?> RequireValidBody<T>(this HttpRequestMessage message)
        {
            if (message.Content == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            var content = await message.Content.ReadAsStringAsync();
            if (content == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            try
            {
                var obj = JsonSerializer.Deserialize<T>(content, JsonSerializerOptions);
                if (obj == null)
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }

                return null;
            }
            catch (JsonException)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        internal static int? GetId(this TestAuthorizationContext testAuthorizationContext)
        {
            if (!testAuthorizationContext.IsAuthenticated)
            {
                return null;
            }

            if (!int.TryParse(testAuthorizationContext.Claims.FirstOrDefault(c => c.Type == AuthConstants.ID_CLAIM_TYPE)?.Value, out var userId))
            {
                return null;
            }

            return userId;
        }
    }
}
