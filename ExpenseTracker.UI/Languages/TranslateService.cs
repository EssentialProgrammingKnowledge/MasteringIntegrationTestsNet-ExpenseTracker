using ExpenseTracker.UI.Models;
using System.Collections.Frozen;
using System.Text;

namespace ExpenseTracker.UI.Languages
{
    public class TranslateService : ITranslateService
    {
        private readonly FrozenDictionary<string, string> _translations = new Dictionary<string, string>()
        {
            { "CATEGORY_LOWER_BUDGET_THAN_TOTAL_EXPENSES", "Nowy budżet '{Budget}' jest niższy niż obecne całkowite wydatki '{TotalExpenses}'. Najpierw zmniejsz wydatki." },
            { "CATEGORY_CANNOT_DELETE_WITH_EXPENSES", "Nie można usunąć kategorii przypisanej do wydatków." },
            { "CATEGORY_NOT_FOUND", "Nie znaleziono kategorii o identyfikatorze '{Id}'." },
            { "EXPENSE_DESCRIPTION_CANNOT_BE_EMPTY", "Opis nie może być pusty." },
            { "EXPENSE_DESCRIPTION_TOO_LONG", "Opis jest za długi ('{CurrentCharactersLength}'). Maksymalna długość to '{MaxCharactersLength}'." },
            { "EXPENSE_AMOUNT_GREATER_THAN_ZERO", "Kwota musi być większa niż zero." },
            { "EXPENSE_NOT_FOUND", "Nie znaleziono wydatku o identyfikatorze '{Id}'." },
            { "EXPENSE_AMOUNT_EXCEEDS_BUDGET", "Dodając kwotę '{Amount}' przekraczono budżet '{Budget}', całkowite wydatki wliczając kwotę '{Amount}' to '{TotalExpenses}'." },
            { "GENERAL_ERROR", "Coś poszło nie tak, spróbuj ponownie później." },
            { "USER_CANNOT_BE_CREATED", "Użytkownik nie może zostać utworzony." },
            { "USER_FIRST_NAME_SHOULD_NOT_BE_EMPTY", "Imię nie może być puste." },
            { "USER_LAST_NAME_SHOULD_NOT_BE_EMPTY", "Nazwisko nie może być puste." },
            { "USER_EMAIL_SHOULD_NOT_BE_EMPTY", "Email nie może być pusty." },
            { "USER_PASSWORD_SHOULD_NOT_BE_EMPTY", "Hasło nie może być puste." },
            { "USER_PASSWORD_NOT_MATCH_POLICY", "Hasło powinno zawierać przynajmniej 1 małą literę, 1 dużą, 1 cyfrę, 1 specjalny znak i długość powinna być większa niż '{MinimalLength}' oraz mniejsza niż '{MaximumLength}'." },
            { "USER_EMAIL_IS_INVALID", "Email '{Email}' jest niepoprawny." },
            { "USER_FIRST_NAME_TOO_SHORT", "Imię powinno zawierać przynajmniej '{MinimalLength}' znaki." },
            { "USER_FIRST_NAME_TOO_LONG", "Imię nie powinno być dłuższe niż '{MaximumLength}' znaków." },
            { "USER_LAST_NAME_TOO_SHORT", "Nazwisko powinno zawierać przynajmniej '{MinimalLength}' znaki." },
            { "USER_LAST_NAME_TOO_LONG", "Nazwisko nie powinno być dłuższe niż '{MaximumLength}' znaków." },
            { "USER_NOT_FOUND_BY_EMAIL", "Użytkownik '{Email}' nie został znaleziony." },
            { "USER_NOT_FOUND_BY_USER_ID", "Użytkownik o numerze id '{UserId}' nie został znaleziony." },
            { "USER_EMAIL_CANNOT_BE_CHANGED", "Email nie może zostać zmieniony." },
            { "USER_INVALID_PASSWORD", "Hasło jest niepoprawne." },
            { "USER_INVALID_CREDENTIALS", "Email lub hasło jest niepoprawne." },
            { "CURRENCY_NOT_SUPPORTED", "Waluta '{Currency}' nie jest obsługiwana." },
            { "CURRENCY_NOT_FOUND", "Waluta '{Currency}' nie została znaleziona." },

            // --- FORM FIELDS ---
            { "RegisterForm.FirstNameControlName", "Imię" },
            { "RegisterForm.LastNameControlName", "Nazwisko" },
            { "RegisterForm.EmailControlName", "Email" },
            { "RegisterForm.PasswordControlName", "Hasło" },
            { "RegisterForm.ConfirmPasswordControlName", "Powtórz hasło" },

            { "Form.FieldRequired", "Pole '{Field}' jest wymagane." },
            { "Form.InvalidEmail", "Pole 'Email' nie zawiera poprawnego adresu email." },
            { "Form.MinimumLength", "Długość pola '{Field}' musi być większa lub równa '{ExpectedLength}' znaki(ów)." },
            { "Form.MaximumLength", "Długość pola '{Field}' musi być mniejsza lub równa '{ExpectedLength}' znaki(ów)." },
            { "Form.PasswordPolicy", "Pole 'Hasło' powinno zawierać przynajmniej 1 dużą literę, 1 małą literę, 1 cyfrę oraz znak specjalny." },
            { "Form.EqualFields", "Pola '{FieldFirst}' i '{FieldSecond}' powinny być takie same." }
        }.ToFrozenDictionary();

        public string Translate(ErrorMessage errorMessage)
        {
            if (errorMessage is null)
            {
                return string.Empty;
            }
            return Translate(errorMessage.Code, errorMessage.Parameters);
        }

        public string Translate(string translationKey, Dictionary<string, object>? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(translationKey))
            {
                return string.Empty;
            }

            _translations.TryGetValue(translationKey, out var translatedText);
            if (translatedText is null)
            {
                return translationKey;
            }

            return ReplaceParameters(translatedText, parameters);
        }

        private string ReplaceParameters(string translatedText, Dictionary<string, object>? parameters = null)
        {
            if (parameters is null || parameters.Count == 0)
            {
                return translatedText;
            }

            var translatedTextWithReplaceParams = new StringBuilder(translatedText);
            foreach (var param in parameters)
            {
                if (param.Key is null || param.Value is null)
                {
                    continue;
                }

                translatedTextWithReplaceParams.Replace($"{{{param.Key}}}", param.Value?.ToString() ?? "null");
            }

            return translatedTextWithReplaceParams.ToString();
        }
    }
}
