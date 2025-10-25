namespace ExpenseTracker.API.DTO
{
    public record CreateUserDTO(
        string FirstName,
        string LastName,
        string Email,
        string Password
    );
}
