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
    public class CategoriesController
        (
            ICategoryService categoryService
        ) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<CategoryDTO>> GetCategories()
        {
            var userId = User.GetUserId();
            return await categoryService.GetAllCategories(userId);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            var userId = User.GetUserId();
            return (await categoryService.GetCategoryByIdAndUserId(id, userId))
                            .ToActionResult();
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDTO>> CreateCategory(CategoryDTO dto)
        {
            var userId = User.GetUserId();
            var result = await categoryService.AddCategory(dto, userId);
            return result.ToCreatedActionResult(this, nameof(GetCategory), new { result.Data!.Id });
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoryDTO>> UpdateCategory(int id, CategoryDTO dto)
        {
            var userId = User.GetUserId();
            return (await categoryService.UpdateCategory(dto with { Id = id }, userId))
                            .ToActionResult();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var userId = User.GetUserId();
            return (await categoryService.DeleteCategory(id, userId))
                            .ToActionResult();
        }
    }
}
