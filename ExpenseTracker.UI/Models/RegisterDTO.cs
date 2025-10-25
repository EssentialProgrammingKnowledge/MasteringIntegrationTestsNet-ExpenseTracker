namespace ExpenseTracker.UI.Models
{
    public record RegisterDTO(
        string FirstName,
        string LastName,
        string Email,
        string Password
    );

    public record RegisterResultDTO(
        string AccessToken
    );
}
