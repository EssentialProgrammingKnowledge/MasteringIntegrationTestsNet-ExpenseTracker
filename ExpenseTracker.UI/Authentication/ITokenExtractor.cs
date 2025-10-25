using System.Security.Claims;

namespace ExpenseTracker.UI.Authentication
{
    public interface ITokenExtractor
    {
        IEnumerable<Claim> GetUserClaims(string? token);
    }
}
