using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Mappings;
using ExpenseTracker.API.Services;

namespace ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController
        (
            IAuthService authService
        )
        : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResultDTO>> Login(LoginDTO dto)
        {
            return (await authService.Login(dto)).ToActionResult();
        }

        [Authorize]
        [HttpGet("validate")]
        public ActionResult Validate()
        {
            return Ok();
        }
    }
}
