using Bunit.TestDoubles;
using ExpenseTracker.IntegrationTests.Setup.Authentication;
using ExpenseTracker.IntegrationTests.Setup.DataStore;
using ExpenseTracker.IntegrationTests.Setup.DataStore.Models;
using ExpenseTracker.UI.Authentication;
using ExpenseTracker.UI.Models;
using RichardSzalay.MockHttp;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ExpenseTracker.IntegrationTests.Setup.InMemoryApi.EndpointsDefintions
{
    internal static class RegisterEndpoints
    {
        private readonly static JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static void AddRegisterEndpoints(this MockHttpMessageHandler mockHttp, IInMemoryDataStore store, TestAuthorizationContext testAuthorizationContext)
        {
            mockHttp.When(HttpMethod.Post, "/api/register")
                .Respond(async message =>
                {
                    var result = await message.RequireValidBody<RegisterDTO>();
                    if (result is not null)
                    {
                        return result;
                    }

                    var dto = await message.Content!.ReadFromJsonAsync<RegisterDTO>(options: JsonSerializerOptions);
                    if (dto is null || store.Users.Any(u => u.Value.Email.Equals(dto?.Email.ToLower(), StringComparison.CurrentCultureIgnoreCase)))
                    {
                        return new HttpResponseMessage(HttpStatusCode.BadRequest)
                        {
                            Content = new StringContent(
                                                JsonSerializer.Serialize(new ErrorMessage("USER_CANNOT_BE_CREATED", "User cannot be created.")),
                                                Encoding.UTF8, "application/json")
                        };
                    }

                    var id = store.Users.Keys.LastOrDefault() + 1;
                    var user = new InMemoryUser() { Id = id, Email = dto.Email, Password = dto.Password, FirstName = dto.FirstName, LastName = dto.LastName, FullName = $"{dto.FirstName} {dto.LastName}", UserId = Guid.NewGuid() };
                    store.Users.Add(id, user);

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
