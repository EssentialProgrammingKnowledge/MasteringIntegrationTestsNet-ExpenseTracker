using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Models;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace ExpenseTracker.API.IntegrationTests.TestServices
{
    internal sealed class ExpenseTestService
        (
            HttpClient httpClient
        )
    {
        private const string URL = "/api/expenses";

        public async Task<ExpenseDetailsDTO> AddExpense(int categoryId, decimal amount = 100M, string ? description = null, Currency currency = Currency.PLN, decimal rate = decimal.One)
        {
            var response = await httpClient.PostAsJsonAsync(URL, new ExpenseDTO(0, amount, categoryId, description ?? "Desc123456789", currency, rate ));
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            var dto = await response.Content.ReadFromJsonAsync<ExpenseDetailsDTO>();
            dto.ShouldNotBeNull();
            return dto;
        }
    }
}
