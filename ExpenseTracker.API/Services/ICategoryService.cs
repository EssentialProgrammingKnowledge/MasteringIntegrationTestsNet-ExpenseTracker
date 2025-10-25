using ExpenseTracker.API.DTO;

namespace ExpenseTracker.API.Services
{
    public interface ICategoryService
    {
        Task<Result<CategoryDTO>> AddCategory(CategoryDTO categoryDto, int userId);
        Task<Result<CategoryDTO>> UpdateCategory(CategoryDTO categoryDto, int userId);
        Task<Result> DeleteCategory(int id, int userId);
        Task<Result<CategoryDTO>> GetCategoryByIdAndUserId(int id, int userId);
        Task<IEnumerable<CategoryDTO>> GetAllCategories(int userId);
    }
}
