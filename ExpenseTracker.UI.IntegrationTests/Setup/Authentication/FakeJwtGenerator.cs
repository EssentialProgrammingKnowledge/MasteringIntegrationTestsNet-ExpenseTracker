using System.Text;
using System.Text.Json;

namespace ExpenseTracker.IntegrationTests.Setup.Authentication
{
    public static class FakeJwtGenerator
    {
        private const string Header = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9";
        private const string Footer = "dummy-signature";

        public static string GenerateToken(this object payload)
        {
            var json = JsonSerializer.Serialize(payload);
            var bytes = Encoding.UTF8.GetBytes(json);
            var base64 = Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
            return $"{Header}.{base64}.{Footer}";
        }
    }
}
