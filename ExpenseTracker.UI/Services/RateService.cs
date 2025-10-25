using ExpenseTracker.UI.Models;
using System.Net.Http.Json;

namespace ExpenseTracker.UI.Services
{
    public class RateService
        (
            HttpClient httpClient
        )
        : IRateService
    {
        private const string PATH = "/api/rates";

        public async Task<Result<List<RateDTO>>> GetAll()
        {
            var response = await httpClient.GetAsync(PATH);
            if (!response.IsSuccessStatusCode)
            {
                return Result<List<RateDTO>>.Failed(await response.ToErrorMessage());
            }
            return Result<List<RateDTO>>.Success(await response.Content.ReadFromJsonAsync<List<RateDTO>>() ?? []);
        }
    }
}
