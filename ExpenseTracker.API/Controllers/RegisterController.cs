using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Mappings;
using ExpenseTracker.API.Services;

namespace ExpenseTracker.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController
        (
            IRegistrationService registrationService
        )
        : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<RegisterResultDTO>> Register(RegisterDTO dto)
        {
            return (await registrationService.Register(dto))
                                     .ToActionResult();
        }
    }
}
