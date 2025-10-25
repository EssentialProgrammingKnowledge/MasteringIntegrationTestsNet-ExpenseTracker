using ExpenseTracker.API.DTO;

namespace ExpenseTracker.API.Services
{
    public interface IUserService
    {
        Task<Result<UserDetailsDTO>> AddUser(CreateUserDTO productDto);
        Task<Result<UserDTO>> GetUserById(int id);
    }
}
