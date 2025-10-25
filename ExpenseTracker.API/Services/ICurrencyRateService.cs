using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Validations;
using Microsoft.Extensions.Caching.Memory;

namespace ExpenseTracker.API.Services
{
    public interface ICurrencyRateService
    {
        Task<Result<CurrencyRateDTO>> GetRate(Currency currency);
        Task<Result<List<CurrencyRateDTO>>> GetRates();
    }

    public class CurrencyRateService
        (
            INbpRatesService nbpRatesService,
            IMemoryCache memoryCache
        )
        : ICurrencyRateService
    {
        private const string CURRENCIES_KEY = "currencies";

        public async Task<Result<CurrencyRateDTO>> GetRate(Currency currency)
        {
            if (!Enum.IsDefined(currency))
            {
                return Result<CurrencyRateDTO>.BadRequestResult(CurrencyRateErrorMessages.CurrencyNotSupported(currency));
            }

            if (currency == Currency.PLN)
            {
                return ReturnOk(currency, decimal.One, DateOnly.FromDateTime(DateTime.UtcNow));
            }

            var existingRate = GetFromCache(currency);
            if (existingRate is not null)
            {
                return ReturnOk(currency, existingRate.Mid, existingRate.EffectiveDate);
            }

            var rates = await UpdateRates();
            var rate = rates.FirstOrDefault(r => r.Currency == currency);
            if (rate is null)
            {
                return Result<CurrencyRateDTO>.NotFoundResult(CurrencyRateErrorMessages.CurrencyNotFound(currency));
            }

            memoryCache.Set(currency, rates, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(15)
            });
            return ReturnOk(currency, rate.Rate, rate.RateDate);
        }

        public async Task<Result<List<CurrencyRateDTO>>> GetRates()
        {
            var currencies = Enum.GetValues<Currency>();
            if (!memoryCache.TryGetValue<List<NbpRateDTO>>(CURRENCIES_KEY, out var existingRates) || existingRates is null || existingRates.Count == 0)
            {
                var result = await UpdateRates();
                return Result<List<CurrencyRateDTO>>.OkResult(result);
            }

            return Result<List<CurrencyRateDTO>>.OkResult(MapToCurrencyRates(existingRates));
        }

        private async Task<List<CurrencyRateDTO>> UpdateRates()
        {
            var rates = await nbpRatesService.GetRatesAsync();
            if (!rates.Any())
            {
                return [];
            }

            var currienciesName = Enum.GetNames<Currency>();
            var filteredCurrencies = rates.Where(r => Enum.TryParse<Currency>(r.Code, true, out _))
                                          .ToList();

            memoryCache.Set(CURRENCIES_KEY, filteredCurrencies, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(15)
            });
            return MapToCurrencyRates(filteredCurrencies);
        }

        private NbpRateDTO? GetFromCache(Currency currency)
        {
            if (!memoryCache.TryGetValue<List<NbpRateDTO>>(CURRENCIES_KEY, out var existingRates) || existingRates is null || existingRates.Count == 0)
            {
                return null;
            }

            return existingRates.FirstOrDefault(r => string.Equals(r.Code, currency.ToString(), StringComparison.InvariantCultureIgnoreCase));
        }

        private Result<CurrencyRateDTO> ReturnOk(Currency currency, decimal rate, DateOnly rateDate)
            => Result<CurrencyRateDTO>.OkResult(new CurrencyRateDTO(currency, rate, rateDate));

        private List<CurrencyRateDTO> MapToCurrencyRates(List<NbpRateDTO> rates)
        {
            return [.. rates.Select(c => new CurrencyRateDTO(Enum.Parse<Currency>(c.Code, true), c.Mid, c.EffectiveDate))];
        }
    }
}
