using Bunit.TestDoubles;
using ExpenseTracker.IntegrationTests.Setup.Authentication;
using ExpenseTracker.IntegrationTests.Setup.DataStore;
using ExpenseTracker.UI.Authentication;
using ExpenseTracker.UI.Models;
using RichardSzalay.MockHttp;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ExpenseTracker.IntegrationTests.Setup.InMemoryApi.EndpointsDefintions
{
    internal static class LoginEndpoints
    {
        private readonly static JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static void AddLoginEndpoints(this MockHttpMessageHandler mockHttp, IInMemoryDataStore store, TestAuthorizationContext testAuthorizationContext)
        {
            mockHttp.When(HttpMethod.Post, "/api/auth/login")
                .Respond(async message =>
                {
                    var result = await message.RequireValidBody<LoginDTO>();
                    if (result is not null)
                    {
                        return result;
                    }

                    var dto = await message.Content!.ReadFromJsonAsync<LoginDTO>(options: JsonSerializerOptions);
                    var user = store.Users.FirstOrDefault(u => u.Value.Email == dto?.Email && u.Value.Password == dto?.Password).Value;
                    if (user == null)
                    {
                        return new HttpResponseMessage(HttpStatusCode.Unauthorized)
                        {
                            Content = new StringContent(
                                                JsonSerializer.Serialize(new ErrorMessage("USER_INVALID_CREDENTIALS", "Invalid credentials.")),
                                                Encoding.UTF8, "application/json")
                        };
                    }

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonSerializer.Serialize(new Dictionary<string, object>
                        {
                            { "AccessToken", new Dictionary<string, string> {
                                    { AuthConstants.ID_CLAIM_TYPE, user.Id.ToString() },
                                    { AuthConstants.EMAIL_CLAIM_TYPE, user.Email },
                                    { AuthConstants.FIRST_NAME_CLAIM_TYPE, user.FirstName },
                                    { AuthConstants.LAST_NAME_CLAIM_TYPE, user.LastName },
                                    { AuthConstants.FULL_NAME_CLAIM_TYPE, user.FullName },
                                    { AuthConstants.USER_ID_CLAIM_TYPE, user.UserId.ToString() }
                                }.GenerateToken() }
                        }),
                        Encoding.UTF8, "application/json")
                    };
                });
        }
    }
}
