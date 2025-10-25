using FluentValidation;
using ExpenseTracker.UI.Languages;

namespace ExpenseTracker.UI.Models
{
    public record RegisterForm
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;

        public RegisterDTO ToDto()
        {
            return new RegisterDTO(FirstName, LastName, Email, Password);
        }
    }

    public class RegisterFormValidator : AbstractValidator<RegisterForm>
    {
        public RegisterFormValidator(ITranslateService translateService)
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(translateService.Translate("Form.FieldRequired", new Dictionary<string, object>
                {
                    { "Field", translateService.Translate("RegisterForm.FirstNameControlName") }
                }));
            RuleFor(x => x.LastName).NotEmpty().WithMessage(translateService.Translate("Form.FieldRequired", new Dictionary<string, object>
                {
                    { "Field", translateService.Translate("RegisterForm.LastNameControlName") }
                }));
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(translateService.Translate("Form.FieldRequired", new Dictionary<string, object>
                {
                    { "Field", translateService.Translate("RegisterForm.EmailControlName") }
                }))
                .EmailAddress().WithMessage(translateService.Translate("Form.InvalidEmail"));

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(translateService.Translate("Form.FieldRequired", new Dictionary<string, object>
                {
                    { "Field", translateService.Translate("RegisterForm.PasswordControlName") }
                }))
                .MinimumLength(12).WithMessage(translateService.Translate("Form.MinimumLength", new Dictionary<string, object>
                {
                    { "Field", translateService.Translate("RegisterForm.PasswordControlName") },
                    { "ExpectedLength", 12 }
                }))
                .MaximumLength(64).WithMessage(translateService.Translate("Form.MaximumLength", new Dictionary<string, object>
                {
                    { "Field", translateService.Translate("RegisterForm.PasswordControlName") },
                    { "ExpectedLength", 64 }
                }))
                .Matches(@"^(?=.*\p{Lu})(?=.*\p{Ll})(?=.*\d)(?=.*\W)").WithMessage(translateService.Translate("Form.PasswordPolicy"));

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password)
                .WithMessage(translateService.Translate("Form.EqualFields", new Dictionary<string, object>
                {
                    { "FieldFirst", translateService.Translate("RegisterForm.PasswordControlName") },
                    { "FieldSecond", translateService.Translate("RegisterForm.ConfirmPasswordControlName") }
                }));
        }
    }
}
