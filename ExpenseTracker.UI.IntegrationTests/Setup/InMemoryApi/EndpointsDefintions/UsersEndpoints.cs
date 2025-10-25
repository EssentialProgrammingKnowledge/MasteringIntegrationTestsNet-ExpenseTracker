using Bunit.TestDoubles;
using ExpenseTracker.IntegrationTests.Setup.DataStore;
using RichardSzalay.MockHttp;
using System.Net;
using System.Net.Http.Json;

namespace ExpenseTracker.IntegrationTests.Setup.InMemoryApi.EndpointsDefintions
{
    internal static class UsersEndpoints
    {
        public static void AddUsersEndpoints(this MockHttpMessageHandler mockHttp, IInMemoryDataStore store, TestAuthorizationContext testAuthorizationContext)
        {
            mockHttp.When(HttpMethod.Get, "/api/users/me")
                .Respond(message =>
                {
                    var result = message.RequireAuthorization(testAuthorizationContext);
                    if (result is not null)
                    {
                        return result;
                    }

                    var id = testAuthorizationContext.GetId();
                    if (!id.HasValue)
                    {
                        return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    }

                    store.Users.TryGetValue(id.Value, out var user);
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = JsonContent.Create(user)
                    };
                });
        }
    }
}
