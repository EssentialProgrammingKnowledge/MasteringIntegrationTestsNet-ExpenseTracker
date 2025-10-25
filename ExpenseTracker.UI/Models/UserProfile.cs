namespace ExpenseTracker.UI.Models
{
    public record UserProfile(
        int Id,
        Guid UserId,
        string Email,
        string FirstName,
        string LastName,
        string FullName
    )
    {
        public UserProfile(UserProfile userProfile)
        {
            Id = userProfile.Id;
            UserId = userProfile.UserId;
            Email = userProfile.Email;
            FirstName = userProfile.FirstName;
            LastName = userProfile.LastName;
            FullName = userProfile.FullName;
        }
    }
}
