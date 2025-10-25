using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Mappings
{
    public static class UserExtensions
    {
        public static UserDTO AsDto(this User user)
        {
            return new UserDTO(
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email
            );
        }

        public static UserDetailsDTO AsDetailsDto(this User user)
        {
            return new UserDetailsDTO(
                user.Id,
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email
            );
        }

        public static JwtUserDataDTO AsJwtUserData(this User user)
        {
            return new JwtUserDataDTO(
                user.Id,
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email
            );
        }

        public static JwtUserDataDTO AsJwtUserData(this UserDetailsDTO user)
        {
            return new JwtUserDataDTO(
                user.Id,
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email
            );
        }
    }
}
