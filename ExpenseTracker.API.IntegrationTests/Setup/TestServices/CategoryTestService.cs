using ExpenseTracker.API.DTO;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace ExpenseTracker.API.IntegrationTests.TestServices
{
    internal sealed class CategoryTestService
        (
            HttpClient httpClient
        )
    {
        private const string URL = "/api/categories";

        public async Task<CategoryDTO> AddCategory(string? name = null, decimal? budget = null)
        {
            var response = await httpClient.PostAsJsonAsync(URL, new CategoryDTO(0, name ?? "Category#123", budget ?? 100M));
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            var dto = await response.Content.ReadFromJsonAsync<CategoryDTO>();
            dto.ShouldNotBeNull();
            return dto;
        }
    }
}
