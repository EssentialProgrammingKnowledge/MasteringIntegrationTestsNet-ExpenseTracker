namespace ExpenseTracker.API.DTO
{
    public record JwtUserDataDTO(
        int Id,
        Guid UserId,
        string FirstName,
        string LastName,
        string Email
    );
}
