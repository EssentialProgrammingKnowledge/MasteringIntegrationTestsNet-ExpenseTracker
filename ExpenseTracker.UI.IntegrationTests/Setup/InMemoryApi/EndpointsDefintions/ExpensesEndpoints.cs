using Bunit.TestDoubles;
using ExpenseTracker.IntegrationTests.Setup.DataStore;
using ExpenseTracker.IntegrationTests.Setup.DataStore.Models;
using ExpenseTracker.UI.Models;
using RichardSzalay.MockHttp;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ExpenseTracker.IntegrationTests.Setup.InMemoryApi.EndpointsDefintions
{
    internal static class ExpensesEndpoints
    {
        private readonly static JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static void AddExpensesEndpoints(this MockHttpMessageHandler mockHttp, IInMemoryDataStore store, TestAuthorizationContext testAuthorizationContext)
        {
            mockHttp.When(HttpMethod.Get, "/api/expenses")
                .Respond(message =>
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(
                                                JsonSerializer.Serialize(store.Expenses.Values
                                                    .Where(e => e.UserId == testAuthorizationContext.GetId()), JsonSerializerOptions),
                                                Encoding.UTF8, "application/json")
                    };
                });

            mockHttp.When(HttpMethod.Get, "/api/expenses/*")
                .Respond(message =>
                {
                    var id = int.Parse(message.RequestUri!.Segments.LastOrDefault() ?? string.Empty);
                    var result = message.RequireAuthorization(testAuthorizationContext);
                    if (result is not null)
                    {
                        return result;
                    }

                    if (!store.Expenses.TryGetValue(id, out var expense) || expense.UserId != testAuthorizationContext.GetId())
                    {
                        return new HttpResponseMessage(HttpStatusCode.NotFound);
                    }
                    if (!store.Categories.TryGetValue(expense.CategoryId, out var category))
                    {
                        return new HttpResponseMessage(HttpStatusCode.Conflict);
                    }

                    var expenseDetails = new ExpenseDetailsDTO
                    {
                        Id = expense.Id,
                        Description = expense.Description,
                        Amount = expense.Amount,
                        Currency = expense.Currency,
                        Rate = expense.Rate,
                        Category = new CategoryDTO
                        {
                            Id = category.Id,
                            Name = category.Name,
                            Budget = category.Budget
                        }
                    };
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = JsonContent.Create(expenseDetails, options: JsonSerializerOptions)
                    };
                });

            mockHttp.When(HttpMethod.Post, "/api/expenses")
                .Respond(async message =>
                {
                    var validBodyResult = await message.RequireValidBody<InMemoryExpense>();
                    if (validBodyResult is not null)
                    {
                        return validBodyResult;
                    }

                    var result = message.RequireAuthorization(testAuthorizationContext);
                    if (result is not null)
                    {
                        return result;
                    }

                    var dto = await message.Content!.ReadFromJsonAsync<InMemoryExpense>(options: JsonSerializerOptions);
                    var userId = testAuthorizationContext.GetId();
                    if (dto is null || !userId.HasValue)
                    {
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);
                    }

                    if (!store.Categories.ContainsKey(dto.CategoryId))
                    {
                        return new HttpResponseMessage(HttpStatusCode.BadRequest)
                        {
                            Content = new StringContent(
                                                JsonSerializer.Serialize(new ErrorMessage("CATEGORY_NOT_FOUND", $"Category with id '{dto.CategoryId}' was not found.", new Dictionary<string, object> { { "Id", dto.CategoryId } })),
                                                Encoding.UTF8, "application/json")
                        };
                    }

                    var id = store.Expenses.Keys.DefaultIfEmpty(0).Max() + 1;
                    dto.Id = id;
                    dto.UserId = userId.Value;
                    store.Expenses.Add(id, dto);
                    return new HttpResponseMessage(HttpStatusCode.Created)
                    {
                        Content = JsonContent.Create(dto)
                    };
                });

            mockHttp.When(HttpMethod.Put, "/api/expenses/*")
                .Respond(async message =>
                {
                    var id = int.Parse(message.RequestUri!.Segments.LastOrDefault() ?? string.Empty);
                    var validBodyResult = await message.RequireValidBody<InMemoryExpense>();
                    if (validBodyResult is not null)
                    {
                        return validBodyResult;
                    }

                    var result = message.RequireAuthorization(testAuthorizationContext);
                    if (result is not null)
                    {
                        return result;
                    }

                    var dto = await message.Content!.ReadFromJsonAsync<InMemoryExpense>(options: JsonSerializerOptions);
                    if (dto is null)
                    {
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);
                    }

                    if (!store.Categories.ContainsKey(dto.CategoryId))
                    {
                        return new HttpResponseMessage(HttpStatusCode.BadRequest)
                        {
                            Content = new StringContent(
                                                JsonSerializer.Serialize(new ErrorMessage("CATEGORY_NOT_FOUND", $"Category with id '{dto.CategoryId}' was not found.", new Dictionary<string, object> { { "Id", dto.CategoryId } })),
                                                Encoding.UTF8, "application/json")
                        };
                    }

                    if (!store.Expenses.TryGetValue(id, out var expense) || expense.UserId != testAuthorizationContext.GetId())
                    {
                        return new HttpResponseMessage(HttpStatusCode.NotFound);
                    }

                    expense.Amount = dto.Amount;
                    expense.Description = dto.Description;
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = JsonContent.Create(expense)
                    };
                });

            mockHttp.When(HttpMethod.Delete, "/api/expenses/*")
                .Respond(req =>
                {
                    var id = int.Parse(req.RequestUri!.Segments.LastOrDefault() ?? string.Empty);
                    if (!store.Expenses.TryGetValue(id, out var expense) || expense.UserId != testAuthorizationContext.GetId())
                    {
                        return new HttpResponseMessage(HttpStatusCode.NotFound);
                    }

                    return new HttpResponseMessage(store.Expenses.Remove(id) ? HttpStatusCode.NoContent : HttpStatusCode.NotFound);
                });
        }
    }
}
