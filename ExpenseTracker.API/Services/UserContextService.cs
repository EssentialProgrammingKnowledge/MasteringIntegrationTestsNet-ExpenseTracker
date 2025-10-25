using ExpenseTracker.API.Security;

namespace ExpenseTracker.API.Services
{
    public class UserContextService : IUserContextService
    {
        public int Id { get; }
        public Guid UserId { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string FullName { get; }

        public UserContextService(IHttpContextAccessor httpContextAccessor, ILogger<UserContextService> logger)
        {
            if (!int.TryParse(httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == JwtClaimConstants.SUB)?.Value ?? string.Empty, out var id))
            {
                logger.LogError("Token has missing claim {Claim}", JwtClaimConstants.SUB);
            }
            Id = id;

            if (!Guid.TryParse(httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == JwtClaimConstants.USER_ID)?.Value ?? string.Empty, out var userId))
            {
                logger.LogError("Token has missing claim {Claim}", JwtClaimConstants.USER_ID);
            }
            UserId = userId;

            if (!AssignString(httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == JwtClaimConstants.EMAIL)?.Value, out var email))
            {
                logger.LogError("Token has missing claim {Claim}", JwtClaimConstants.EMAIL);
            }
            Email = email;

            if (!AssignString(httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == JwtClaimConstants.GIVEN_NAME)?.Value, out var firstName))
            {
                logger.LogError("Token has missing claim {Claim}", JwtClaimConstants.GIVEN_NAME);
            }
            FirstName = firstName;

            if (!AssignString(httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == JwtClaimConstants.FAMILY_NAME)?.Value, out var lastName))
            {
                logger.LogError("Token has missing claim {Claim}", JwtClaimConstants.FAMILY_NAME);
            }
            LastName = lastName;

            if (!AssignString(httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == JwtClaimConstants.NAME)?.Value, out var fullName))
            {
                logger.LogError("Token has missing claim {Claim}", JwtClaimConstants.NAME);
            }
            FullName = fullName;
        }

        private bool AssignString(string? value, out string assignValue)
        {
            assignValue = string.Empty;
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            assignValue = value;
            return true;
        }
    }
}
