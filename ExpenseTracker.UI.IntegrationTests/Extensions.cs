using AngleSharp.Dom;
using Bunit;
using ExpenseTracker.UI.Authentication;
using ExpenseTracker.UI.Models;
using System.Security.Claims;

namespace ExpenseTracker.UI.IntegrationTests
{
    internal static class Extensions
    {
        public static IElement? FindFirstOrDefault(this IRenderedFragment fragment, string selector)
        {
            var elements = fragment.FindAll(selector);
            return elements.Count > 0 ? elements[0] : null;
        }

        public static Claim[] CreateClaims(this UserProfile userProfile)
        {
            return [
                new Claim(AuthConstants.ID_CLAIM_TYPE, userProfile.Id.ToString()),
                new Claim(AuthConstants.EMAIL_CLAIM_TYPE, userProfile.Email),
                new Claim(AuthConstants.FIRST_NAME_CLAIM_TYPE, userProfile.FirstName),
                new Claim(AuthConstants.LAST_NAME_CLAIM_TYPE, userProfile.LastName),
                new Claim(AuthConstants.NAME_CLAIM_TYPE, userProfile.FullName),
                new Claim(AuthConstants.USER_ID_CLAIM_TYPE, userProfile.UserId.ToString())
            ];
        }
    }
}
