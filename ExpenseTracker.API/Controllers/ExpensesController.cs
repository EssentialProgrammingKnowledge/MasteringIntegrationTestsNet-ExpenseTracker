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
    public class ExpensesController
        (
            IExpenseService expenseService
        )
        : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<ExpenseDTO>> GetExpenses()
        {
            var userId = User.GetUserId();
            return await expenseService.GetAllExpenses(userId);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<ExpenseDetailsDTO>> GetExpense(int id)
        {
            var userId = User.GetUserId();
            return (await expenseService.GetExpenseById(id, userId)).ToActionResult();
        }

        [HttpPost]
        public async Task<ActionResult<ExpenseDetailsDTO>> AddExpense(ExpenseDTO expense)
        {
            var userId = User.GetUserId();
            var result = await expenseService.AddExpense(expense, userId);
            return result.ToCreatedActionResult(this, nameof(GetExpense), new { result.Data?.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ExpenseDetailsDTO>> UpdateExpense(int id, ExpenseDTO expense)
        {
            var userId = User.GetUserId();
            return (await expenseService.UpdateExpense(expense with { Id = id }, userId))
                .ToActionResult();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteExpense(int id)
        {
            var userId = User.GetUserId();
            return (await expenseService.DeleteExpense(id, userId))
                .ToActionResult();
        }
    }
}
