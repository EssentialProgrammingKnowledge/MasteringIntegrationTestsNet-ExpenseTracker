using ExpenseTracker.UI.Models;
using System.Net.Http.Json;

namespace ExpenseTracker.UI.Services
{
    public class AuthService
        (
            HttpClient httpClient
        ) : IAuthService
    {
        private const string PATH = "/api/auth";

        public async Task<Result<LoginResultDTO>> Login(LoginDTO dto)
        {
            var response = await httpClient.PostAsJsonAsync($"{PATH}/login", dto);
            if (!response.IsSuccessStatusCode)
            {
                return Result<LoginResultDTO>.Failed(await response.ToErrorMessage());
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResultDTO>();
            return Result<LoginResultDTO>.Success(result);
        }

        public async Task<Result> Validate()
        {
            var response = await httpClient.GetAsync($"{PATH}/validate");
            if (!response.IsSuccessStatusCode)
            {
                return Result.Failed(new ErrorMessage("UNAUTHORIZED", "User is not authorized."));
            }

            return Result.Success();
        }
    }
}
