using ExpenseTracker.API.Security;
using System.Security.Claims;

namespace ExpenseTracker.API.IntegrationTests.Setup.Auth
{
    public class TestUserAccessor
    {
        public TestUser? CurrentUser { get; private set; }

        public void SetUser(TestUser user) => CurrentUser = user;
        public void ClearUser() => CurrentUser = null;
    }

    public class TestUser
    {
        public IEnumerable<Claim> Claims => _claimsCollection;
        private readonly List<Claim> _claimsCollection = [];

        public TestUser SetId(string id)
        {
            _claimsCollection.RemoveAll(c => c.Type == JwtClaimConstants.SUB);
            _claimsCollection.Add(new Claim(JwtClaimConstants.SUB, id));
            return this;
        }

        public TestUser SetEmail(string email)
        {
            _claimsCollection.RemoveAll(c => c.Type == JwtClaimConstants.EMAIL);
            _claimsCollection.Add(new Claim(JwtClaimConstants.EMAIL, email));
            return this;
        }

        public TestUser SetName(string username)
        {
            _claimsCollection.RemoveAll(c => c.Type == JwtClaimConstants.NAME);
            _claimsCollection.Add(new Claim(JwtClaimConstants.NAME, username));
            return this;
        }

        public TestUser SetUserId(string userId)
        {
            _claimsCollection.RemoveAll(c => c.Type == JwtClaimConstants.USER_ID);
            _claimsCollection.Add(new Claim(JwtClaimConstants.USER_ID, userId));
            return this;
        }

        public TestUser SetClaims(IEnumerable<Claim> claims)
        {
            _claimsCollection.Clear();
            _claimsCollection.AddRange(claims ?? []);
            return this;
        }

        public static TestUser Admin(string id) => new TestUser().SetName("AdminUser").SetId(id);
        public static TestUser User(string id) => new TestUser().SetName("NormalUser").SetId(id);
    }
}
