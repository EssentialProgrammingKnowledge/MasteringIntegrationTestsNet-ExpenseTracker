namespace ExpenseTracker.API.DTO
{
    public record RegisterDTO(
        string FirstName,
        string LastName,
        string Email,
        string Password
    )
    {
        public CreateUserDTO ToCreateUserDto()
        {
            return new CreateUserDTO(
                FirstName,
                LastName,
                Email,
                Password
            );
        }
    }
}
