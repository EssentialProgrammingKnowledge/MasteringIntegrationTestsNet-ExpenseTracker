using ExpenseTracker.API.DTO;

namespace ExpenseTracker.API.Services
{
    public class NbpRatesService : INbpRatesService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public NbpRatesService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<NbpRateDTO>> GetRatesAsync()
        {
            var client = _httpClientFactory.CreateClient("NbpClient");
            var tables = await client.GetFromJsonAsync<List<NbpTableDTO>>("api/exchangerates/tables/A/?format=json");

            var tableRates = tables?.FirstOrDefault();
            if (tableRates is null)
            {
                return [];
            }

            if (tableRates.Rates.Count == 0)
            {
                return [];
            }

            return tableRates.Rates.Select(r => new NbpRateDTO
            {
                Code = r.Code,
                Currency = r.Currency,
                EffectiveDate = tableRates.EffectiveDate,
                Mid = r.Mid
            });
        }
    }
}
