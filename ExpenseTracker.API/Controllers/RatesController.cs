using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Mappings;
using ExpenseTracker.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RatesController
        (
            ICurrencyRateService currencyRateService
        )
        : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<CurrencyRateDTO>>> GetRates()
        {
            return (await currencyRateService.GetRates())
                                        .ToActionResult();
        }
    }
}
