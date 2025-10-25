using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Mappings;
using ExpenseTracker.API.Services;

namespace ExpenseTracker.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController
        (
            IUserService userService
        )
        : ControllerBase
    {
        [HttpGet("me")]
        public async Task<ActionResult<UserDTO>> GetMyUser()
        {
            return (await userService.GetUserById(User.GetUserId()))
                                     .ToActionResult();
        }
    }
}
