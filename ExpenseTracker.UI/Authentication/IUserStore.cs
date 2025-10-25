using ExpenseTracker.UI.Models;
using System.Security.Claims;

namespace ExpenseTracker.UI.Authentication
{
    public interface IUserStore
    {
        UserProfile? GetUserProfile();
        void SetUserProfile(IEnumerable<Claim> claims);
        void ClearUserProfile();
    }
}
