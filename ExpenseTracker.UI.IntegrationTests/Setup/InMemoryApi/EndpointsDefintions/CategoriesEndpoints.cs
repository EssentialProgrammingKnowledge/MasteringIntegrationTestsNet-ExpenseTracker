using Bunit.TestDoubles;
using ExpenseTracker.IntegrationTests.Setup.DataStore;
using ExpenseTracker.IntegrationTests.Setup.DataStore.Models;
using RichardSzalay.MockHttp;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ExpenseTracker.IntegrationTests.Setup.InMemoryApi.EndpointsDefintions
{
    internal static class CategoriesEndpoints
    {
        private readonly static JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static void AddCategoriesEndpoints(this MockHttpMessageHandler mockHttp, IInMemoryDataStore store, TestAuthorizationContext testAuthorizationContext)
        {
            mockHttp.When(HttpMethod.Get, "/api/categories")
                .Respond(message =>
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(
                                                JsonSerializer.Serialize(store.Categories.Values
                                                    .Where(c => c.UserId == testAuthorizationContext.GetId()), JsonSerializerOptions),
                                                Encoding.UTF8, "application/json")
                    };
                });

            mockHttp.When(HttpMethod.Get, "/api/categories/*")
                .Respond(message =>
                {
                    var id = int.Parse(message.RequestUri!.Segments.LastOrDefault() ?? string.Empty);
                    var result = message.RequireAuthorization(testAuthorizationContext);
                    if (result is not null)
                    {
                        return result;
                    }

                    if (!store.Categories.TryGetValue(id, out var category) || category.UserId != testAuthorizationContext.GetId())
                    {
                        return new HttpResponseMessage(HttpStatusCode.NotFound);
                    }

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = JsonContent.Create(category, options: JsonSerializerOptions)
                    };
                });

            mockHttp.When(HttpMethod.Post, "/api/categories")
                .Respond(async message =>
                {
                    var validBodyResult = await message.RequireValidBody<InMemoryCategory>();
                    if (validBodyResult is not null)
                    {
                        return validBodyResult;
                    }

                    var result = message.RequireAuthorization(testAuthorizationContext);
                    if (result is not null)
                    {
                        return result;
                    }

                    var dto = await message.Content!.ReadFromJsonAsync<InMemoryCategory>(options: JsonSerializerOptions);
                    var userId = testAuthorizationContext.GetId();
                    if (dto is null || !userId.HasValue)
                    {
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);
                    }

                    var id = store.Categories.Keys.DefaultIfEmpty(0).Max() + 1;
                    dto.Id = id;
                    dto.UserId = userId.Value;
                    store.Categories.Add(id, dto);
                    return new HttpResponseMessage(HttpStatusCode.Created)
                    {
                        Content = JsonContent.Create(dto)
                    };
                });

            mockHttp.When(HttpMethod.Put, "/api/categories/*")
                .Respond(async message =>
                {
                    var id = int.Parse(message.RequestUri!.Segments.LastOrDefault() ?? string.Empty);
                    var validBodyResult = await message.RequireValidBody<InMemoryCategory>();
                    if (validBodyResult is not null)
                    {
                        return validBodyResult;
                    }

                    var result = message.RequireAuthorization(testAuthorizationContext);
                    if (result is not null)
                    {
                        return result;
                    }

                    var dto = await message.Content!.ReadFromJsonAsync<InMemoryCategory>(options: JsonSerializerOptions);
                    if (dto is null)
                    {
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);
                    }

                    if (!store.Categories.TryGetValue(id, out var category) || category.UserId != testAuthorizationContext.GetId())
                    {
                        return new HttpResponseMessage(HttpStatusCode.NotFound);
                    }

                    category.Name = dto.Name;
                    category.Budget = dto.Budget;
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = JsonContent.Create(category)
                    };
                });

            mockHttp.When(HttpMethod.Delete, "/api/categories/*")
                .Respond(req =>
                {
                    var id = int.Parse(req.RequestUri!.Segments.LastOrDefault() ?? string.Empty);
                    if (!store.Categories.TryGetValue(id, out var category) || category.UserId != testAuthorizationContext.GetId())
                    {
                        return new HttpResponseMessage(HttpStatusCode.NotFound);
                    }

                    return new HttpResponseMessage(store.Categories.Remove(id) ? HttpStatusCode.NoContent : HttpStatusCode.NotFound);
                });
        }
    }
}
