using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Validations
{
    public static class CurrencyRateErrorMessages
    {
        public static ErrorMessage CurrencyNotSupported(Currency currency)
        {
            return new ErrorMessage("CURRENCY_NOT_SUPPORTED", $"Currency '{currency}' is not supported.",
                new Dictionary<string, object>
                {
                    { "Currency", currency.ToString() }
                });
        }

        public static ErrorMessage CurrencyNotFound(Currency currency)
        {
            return new ErrorMessage("CURRENCY_NOT_FOUND", $"Currency '{currency}' was not found.",
                new Dictionary<string, object>
                {
                    { "Currency", currency.ToString() }
                });
        }
    }
}
