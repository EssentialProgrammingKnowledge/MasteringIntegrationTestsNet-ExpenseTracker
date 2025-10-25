using System.Security.Claims;
using System.Text.Json;

namespace ExpenseTracker.UI.Authentication
{
    public class TokenExtractor : ITokenExtractor
    {
        public IEnumerable<Claim> GetUserClaims(string? token)
        {
            if (token is null)
            {
                return [];
            }

            var splitToken = token.Split('.');
            if (splitToken.Length < 2)
            {
                return [];
            }

            var payload = token.Split('.')[1];
            var jsonBytes = Convert.FromBase64String(PadBase64(payload));
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs?.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? "")) ?? [];
        }

        private static string PadBase64(string base64)
        {
            if (base64.Length % 4 == 2)
            {
                return base64 + "==";
            }
            else if (base64.Length % 4 == 3)
            {
                return base64 + "=";
            }
            else
            {
                return base64;
            }
        }
    }
}
