using ExpenseTracker.UI.Models;
using System.Security.Claims;

namespace ExpenseTracker.UI.Authentication
{
    public class UserStore : IUserStore
    {
        private UserProfile? userProfile;

        public UserProfile? GetUserProfile()
        {
            return userProfile is not null ?
                new UserProfile(userProfile)
                : null;
        }

        public void SetUserProfile(IEnumerable<Claim> claims)
        {
            _ = int.TryParse(claims.FirstOrDefault(c => c.Type == AuthConstants.ID_CLAIM_TYPE)?.Value ?? "", out var id);
            _ = Guid.TryParse(claims.FirstOrDefault(c => c.Type == AuthConstants.USER_ID_CLAIM_TYPE)?.Value ?? "", out var userId);
            userProfile = new UserProfile(
                id,
                userId,
                claims.FirstOrDefault(c => c.Type == AuthConstants.EMAIL_CLAIM_TYPE)?.Value ?? "",
                claims.FirstOrDefault(c => c.Type == AuthConstants.FIRST_NAME_CLAIM_TYPE)?.Value ?? "",
                claims.FirstOrDefault(c => c.Type == AuthConstants.LAST_NAME_CLAIM_TYPE)?.Value ?? "",
                claims.FirstOrDefault(c => c.Type == AuthConstants.FULL_NAME_CLAIM_TYPE)?.Value ?? ""
            );
        }

        public void ClearUserProfile()
        {
            userProfile = null;
        }
    }
}
