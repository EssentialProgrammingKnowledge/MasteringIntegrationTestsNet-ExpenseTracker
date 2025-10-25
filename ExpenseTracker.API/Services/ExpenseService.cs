using ExpenseTracker.API.DTO;
using ExpenseTracker.API.Mappings;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Repositories;
using ExpenseTracker.API.Validations;

namespace ExpenseTracker.API.Services
{
    internal sealed class ExpenseService
        (
            IExpenseRepository expenseRepository, 
            ICategoryRepository categoryRepository,
            ICurrencyRateService currencyRateService
        ) : IExpenseService
    {
        public async Task<Result<ExpenseDetailsDTO>> AddExpense(ExpenseDTO expenseDto, int userId)
        {
            var result = ValidateExpense(expenseDto);
            if (!result.Success)
            {
                return Result<ExpenseDetailsDTO>.BadRequestResult(result.ErrorMessage!);
            }

            var category = await categoryRepository.GetByIdAndUserId(expenseDto.CategoryId, userId);
            if (category is null)
            {
                return Result<ExpenseDetailsDTO>.BadRequestResult(CategoryErrorMessages.NotFound(expenseDto.CategoryId));
            }

            var currentRateResult = await currencyRateService.GetRate(expenseDto.Currency);
            if (!currentRateResult.Success)
            {
                return Result<ExpenseDetailsDTO>.BadRequestResult(currentRateResult.ErrorMessage!);
            }

            var rate = currentRateResult.Data!.Rate;
            var budgetExceededResult = await ShouldNotExceedTheBudget(category, expenseDto, rate);
            if (!budgetExceededResult.Success)
            {
                return Result<ExpenseDetailsDTO>.BadRequestResult(budgetExceededResult.ErrorMessage!);
            }

            var expense = new Expense
            {
                Amount = expenseDto.Amount,
                CategoryId = expenseDto.CategoryId,
                Description = expenseDto.Description,
                Category = category,
                Rate = rate,
                Currency = expenseDto.Currency,
                UserId = userId
            };
            expense = await expenseRepository.Add(expense);
            return Result<ExpenseDetailsDTO>.CreatedResult(expense.AsDetailsDto());
        }

        public async Task<Result> DeleteExpense(int id, int userId)
        {
            var result = await expenseRepository.DeleteByIdAndUserId(id, userId);
            return result ?
                Result.NoContentResult()
                : Result.NotFoundResult(ExpenseErrorMessages.NotFound(id));
        }

        public async Task<IEnumerable<ExpenseDTO>> GetAllExpenses(int userId)
        {
            return [.. (await expenseRepository.GetAllByUserId(userId)).Select(e => e.AsDto())];
        }

        public async Task<Result<ExpenseDetailsDTO>> GetExpenseById(int id, int userId)
        {
            var expense = await expenseRepository.GetByIdAndUserId(id, userId);
            if (expense is null)
            {
                return Result<ExpenseDetailsDTO>.NotFoundResult(ExpenseErrorMessages.NotFound(id));
            }

            return Result<ExpenseDetailsDTO>.OkResult(expense.AsDetailsDto());
        }

        public async Task<Result<ExpenseDetailsDTO>> UpdateExpense(ExpenseDTO expenseDto, int userId)
        {
            var result = ValidateExpense(expenseDto);
            if (!result.Success)
            {
                return Result<ExpenseDetailsDTO>.BadRequestResult(result.ErrorMessage!);
            }

            var expense = await expenseRepository.GetByIdAndUserId(expenseDto.Id, userId);
            if (expense is null)
            {
                return Result<ExpenseDetailsDTO>.NotFoundResult(ExpenseErrorMessages.NotFound(expenseDto.Id));
            }

            var category = await categoryRepository.GetByIdAndUserId(expenseDto.CategoryId, userId);
            if (category is null)
            {
                return Result<ExpenseDetailsDTO>.BadRequestResult(CategoryErrorMessages.NotFound(expenseDto.CategoryId));
            }

            var budgetExceededResult = await ShouldNotExceedTheBudget(category, expenseDto, expense.Rate, expense);
            if (!budgetExceededResult.Success)
            {
                return Result<ExpenseDetailsDTO>.BadRequestResult(budgetExceededResult.ErrorMessage!);
            }

            expense.Description = expenseDto.Description;
            expense.Amount = expenseDto.Amount;
            expense.CategoryId = expenseDto.CategoryId;
            await expenseRepository.Update(expense);
            return Result<ExpenseDetailsDTO>.OkResult(expense.AsDetailsDto());
        }

        private ValidationResult ValidateExpense(ExpenseDTO expenseDto)
        {
            if (expenseDto.Amount <= 0)
            {
                return ValidationResult.FailureResult(ExpenseErrorMessages.AmountMustBeGreaterThanZero());
            }

            if (string.IsNullOrWhiteSpace(expenseDto.Description))
            {
                return ValidationResult.FailureResult(ExpenseErrorMessages.DescriptionCannotBeEmpty());
            }

            if (expenseDto.Description.Length > 250)
            {
                return ValidationResult.FailureResult(ExpenseErrorMessages.DescriptionTooLong(250, expenseDto.Description.Length));
            }

            return ValidationResult.SuccessResult();
        }

        private async Task<ValidationResult> ShouldNotExceedTheBudget(Category category, ExpenseDTO expenseDto, decimal rate, Expense? expense = null)
        {
            decimal totalExpenses = await expenseRepository.GetTotalExpensesAmount(expenseDto.CategoryId);
            var calculatedAmount = expenseDto.Amount * rate;
            decimal newTotalExpenses = totalExpenses - (expense?.GetTotalAmount() ?? 0) + calculatedAmount;

            if (newTotalExpenses > category.Budget)
            {
                return ValidationResult.FailureResult(ExpenseErrorMessages.AmountExceedsBudget(calculatedAmount, category.Budget, newTotalExpenses));
            }
            return ValidationResult.SuccessResult();
        }
    }
}
