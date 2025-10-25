using Microsoft.AspNetCore.Identity;
using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Mappings;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Repositories;
using ExpenseTracker.API.Validations;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace ExpenseTracker.API.Services
{
    public class UserService
        (
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            ILogger<UserService> logger
        )
        : IUserService
    {
        public async Task<Result<UserDetailsDTO>> AddUser(CreateUserDTO dto)
        {
            var validationResult = ValidateCreateUser(dto);
            if (!validationResult.Success)
            {
                return Result<UserDetailsDTO>.BadRequestResult(validationResult.ErrorMessage!);
            }

            if (await userRepository.ExistsByEmail(dto.Email))
            {
                logger.LogWarning("User with email '{Email}' already exists", dto.Email);
                return Result<UserDetailsDTO>.BadRequestResult(UserErrorMessages.UserCannotBeCreated());
            }

            var user = new User
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };
            user.Password = passwordHasher.HashPassword(user, dto.Password);
            await userRepository.Add(user);
            return Result<UserDetailsDTO>.CreatedResult(user.AsDetailsDto());
        }

        public async Task<Result<UserDTO>> GetUserById(int id)
        {
            var user = await userRepository.GetById(id);
            if (user is null)
            {
                return Result<UserDTO>.UnauthorizeResult();
            }

            return Result<UserDTO>.OkResult(user.AsDto());
        }

        private ValidationResult ValidateCreateUser(CreateUserDTO dto)
        {
            var userDataValidation = ValidateUserData(dto.FirstName, dto.LastName);
            if (!userDataValidation.Success)
            {
                return userDataValidation;
            }

            var emailValidation = ValidateEmail(dto.Email);
            if (!emailValidation.Success)
            {
                return emailValidation;
            }

            var passwordValidation = ValidatePassword(dto.Password);
            if (!passwordValidation.Success)
            {
                return passwordValidation;
            }

            return ValidationResult.SuccessResult();
        }

        private ValidationResult ValidateUserData(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                return ValidationResult.FailureResult(UserErrorMessages.FirstNameShouldNotBeEmpty());
            }

            if (firstName.Length < 2)
            {
                return ValidationResult.FailureResult(UserErrorMessages.FirstNameTooShort(2));

            }

            if (firstName.Length > 100)
            {
                return ValidationResult.FailureResult(UserErrorMessages.FirstNameTooLong(100));
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                return ValidationResult.FailureResult(UserErrorMessages.LastNameShouldNotBeEmpty());
            }

            if (lastName.Length < 2)
            {
                return ValidationResult.FailureResult(UserErrorMessages.LastNameTooShort(2));
            }

            if (lastName.Length > 200)
            {
                return ValidationResult.FailureResult(UserErrorMessages.LastNameTooLong(200));
            }

            return ValidationResult.SuccessResult();
        }

        private ValidationResult ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return ValidationResult.FailureResult(UserErrorMessages.EmailShouldNotBeEmpty());
            }

            try
            {
                var mailAddress = new MailAddress(email);
                return mailAddress.Address == email
                    ? ValidationResult.SuccessResult()
                    : ValidationResult.FailureResult(UserErrorMessages.InvalidEmail(email));;
            }
            catch (Exception exception)
            {
                logger.LogDebug(exception, exception.Message);
                return ValidationResult.FailureResult(UserErrorMessages.InvalidEmail(email));
            }
        }

        private ValidationResult ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return ValidationResult.FailureResult(UserErrorMessages.PasswordShouldNotBeEmpty());
            }

            if (!PasswordValidator.IsValid(password))
            {
                return ValidationResult.FailureResult(UserErrorMessages.PasswordNotMatchPolicy(11, 65));
            }

            return ValidationResult.SuccessResult();
        }
    }

    public static partial class PasswordValidator
    {
        [GeneratedRegex(@"^(?=.*\p{Lu})(?=.*\p{Ll})(?=.*\d)(?=.*\W).{12,64}$",
            RegexOptions.CultureInvariant)]
        private static partial Regex PasswordRegex();

        public static bool IsValid(string password)
            => PasswordRegex().IsMatch(password);
    }
}
