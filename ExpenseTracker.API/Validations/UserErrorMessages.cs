using ExpenseTracker.API.DTO;

namespace ExpenseTracker.API.Validations
{
    public static class UserErrorMessages
    {
        public static ErrorMessage UserCannotBeCreated()
        {
            return new ErrorMessage("USER_CANNOT_BE_CREATED", "User cannot be created.");
        }

        public static ErrorMessage FirstNameShouldNotBeEmpty()
        {
            return new ErrorMessage("USER_FIRST_NAME_SHOULD_NOT_BE_EMPTY", "First name should not be empty.");
        }

        public static ErrorMessage LastNameShouldNotBeEmpty()
        {
            return new ErrorMessage("USER_LAST_NAME_SHOULD_NOT_BE_EMPTY", "Last name should not be empty.");
        }

        public static ErrorMessage EmailShouldNotBeEmpty()
        {
            return new ErrorMessage("USER_EMAIL_SHOULD_NOT_BE_EMPTY", "Email should not be empty.");
        }

        public static ErrorMessage PasswordShouldNotBeEmpty()
        {
            return new ErrorMessage("USER_PASSWORD_SHOULD_NOT_BE_EMPTY", "Password should not be empty.");
        }

        public static ErrorMessage PasswordNotMatchPolicy(int minimalLength, int maximumLength)
        {
            return new ErrorMessage("USER_PASSWORD_NOT_MATCH_POLICY", $"Password should contain at least one lower case letter, one upper case letter, one number, one special character and length should not be less than '{minimalLength}' and greater than '{maximumLength}'.",
                new Dictionary<string, object>
                {
                    { "MinimalLength", minimalLength },
                    { "MaximumLength", maximumLength }
                });
        }

        public static ErrorMessage InvalidEmail(string email)
        {
            return new ErrorMessage("USER_EMAIL_IS_INVALID", $"Email {email} is invalid.",
                new Dictionary<string, object>
                {
                    { "Email", email }
                });
        }

        public static ErrorMessage FirstNameTooShort(int minimalLength)
        {
            return new ErrorMessage("USER_FIRST_NAME_TOO_SHORT", $"First name is too short, should contain at least '{minimalLength}' characters.",
                new Dictionary<string, object>
                {
                    { "MinimalLength", minimalLength }
                });
        }

        public static ErrorMessage FirstNameTooLong(int maximumLength)
        {
            return new ErrorMessage("USER_FIRST_NAME_TOO_LONG", $"First name is too long, should contain at maximum '{maximumLength}' characters.",
                new Dictionary<string, object>
                {
                    { "MaximumLength", maximumLength }
                });
        }

        public static ErrorMessage LastNameTooShort(int minimalLength)
        {
            return new ErrorMessage("USER_LAST_NAME_TOO_SHORT", $"Last name is too short, should contain at least '{minimalLength}' characters.",
                new Dictionary<string, object>
                {
                    { "MinimalLength", minimalLength }
                });
        }

        public static ErrorMessage LastNameTooLong(int maximumLength)
        {
            return new ErrorMessage("USER_LAST_NAME_TOO_LONG", $"Last name is too long, should contain at maximum '{maximumLength}' characters.",
                new Dictionary<string, object>
                {
                    { "MaximumLength", maximumLength }
                });
        }

        public static ErrorMessage InvalidCredentials()
        {
            return new ErrorMessage("USER_INVALID_CREDENTIALS", "Invalid credentials.");
        }
    }
}
