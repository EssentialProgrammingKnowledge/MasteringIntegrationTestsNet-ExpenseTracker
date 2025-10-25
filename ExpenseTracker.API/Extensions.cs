using ExpenseTracker.API.Security;
using System.Globalization;
using System.Security.Claims;

namespace ExpenseTracker.API
{
    public static class Extensions
    {
        public static int GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            var idString = claimsPrincipal.FindFirstValue(JwtClaimConstants.SUB);
            if (!int.TryParse(idString, out var userId))
            {
                return 0;
            }

            return userId;
        }

        internal static decimal RoundUp(this decimal value, int position = 2)
        {
            return Math.Ceiling(value * (decimal)Math.Pow(10, position)) / (decimal)Math.Pow(10, position);
        }

        internal static string ToLeadingZerosString(this decimal value, int position = 2)
        {
            return value.ToString($"0.{new string('0', position)}", CultureInfo.InvariantCulture);
        }
    }
}
