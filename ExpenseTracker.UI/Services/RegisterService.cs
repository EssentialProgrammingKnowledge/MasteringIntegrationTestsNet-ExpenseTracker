using ExpenseTracker.UI.Models;
using System.Net.Http.Json;

namespace ExpenseTracker.UI.Services
{
    public class RegisterService
        (
            HttpClient httpClient
        )
        : IRegisterService
    {
        private const string PATH = "/api/register";

        public async Task<Result<RegisterResultDTO>> Register(RegisterDTO dto)
        {
            var response = await httpClient.PostAsJsonAsync(PATH, dto);
            if (!response.IsSuccessStatusCode)
            {
                return Result<RegisterResultDTO>.Failed(await response.ToErrorMessage());
            }

            var result = await response.Content.ReadFromJsonAsync<RegisterResultDTO>();
            return Result<RegisterResultDTO>.Success(result);
        }
    }
}
