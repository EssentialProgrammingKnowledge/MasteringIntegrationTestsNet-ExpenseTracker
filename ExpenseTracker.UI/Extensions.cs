using ExpenseTracker.UI.Models;
using System.Globalization;
using static MudBlazor.Colors;

namespace ExpenseTracker.UI
{
    public static class Extensions
    {
        private static readonly Dictionary<Currency, string> _map = new()
        {
            { Currency.PLN, "pl-PL" },
            { Currency.USD, "en-US" },
            { Currency.EUR, "de-DE" },
            { Currency.CHF, "fr-CH" }
        };

        public static string ToCurrencyString(this decimal value, Currency currency = Currency.PLN)
        {
            _map.TryGetValue(currency, out var format);
            return value.ToString("C", new CultureInfo(format ?? "pl-PL"));
        }

        public static string ToRoundUpCurrencyString(this decimal value, Currency currency = Currency.PLN)
        {
            return (Math.Ceiling(value * 100) / 100).ToCurrencyString(currency);
        }

        public static decimal FromCurrencyStringToDecimal(this string currencyString, Currency currency = Currency.PLN)
        {
            _map.TryGetValue(currency, out var format);
            _ = decimal.TryParse(currencyString, NumberStyles.Currency, new CultureInfo(format ?? "pl-PL"), out var value);
            return value;
        }
    }
}
