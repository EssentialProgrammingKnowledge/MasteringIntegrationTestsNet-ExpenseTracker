namespace ExpenseTracker.API.DTO
{
    public record UserDTO(
        Guid UserId,
        string FirstName,
        string LastName,
        string Email
    );

    public record UserDetailsDTO(
        int Id,
        Guid UserId,
        string FirstName,
        string LastName,
        string Email
    );
}
