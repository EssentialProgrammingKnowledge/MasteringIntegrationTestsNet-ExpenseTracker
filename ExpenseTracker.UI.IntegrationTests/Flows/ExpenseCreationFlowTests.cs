using Bunit;
using ExpenseTracker.IntegrationTests.Setup;
using ExpenseTracker.IntegrationTests.Setup.Services;
using ExpenseTracker.UI.Authentication;
using ExpenseTracker.UI.IntegrationTests.PageObjects;
using ExpenseTracker.UI.Models;
using ExpenseTracker.UI.Pages;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Security.Claims;

namespace ExpenseTracker.UI.IntegrationTests.Flows
{
    public class ExpenseCreationFlowTests
    {
        private readonly TestFixture _testFixture = new();
        private CategoriesPageObject _categoriesPage;
        private HomePageObject _homePage;

        public ExpenseCreationFlowTests()
        {
            _testFixture.TestContext.AuthContext.SetAuthorized("testowyUser");
            _testFixture.TestContext.AuthContext.SetClaims(new Claim(AuthConstants.ID_CLAIM_TYPE, "1"));
        }

        [Fact]
        public async Task ExpenseCreation_ValidData_ShouldAddCategoryAndExpense()
        {
            // Arrange
            var categoryBudget = 1500M;

            // Act
            var category = await AddCategory(budget: categoryBudget);
            
            // Assert
            category.ShouldNotBeNull();
            category.Name.ShouldNotBeNull();
            category.Budget.ShouldBe(categoryBudget);

            // Arrange
            var amount = 500M;

            // Act
            var expense = await AddExpense(category.Name, amount: amount);

            // Assert
            expense.ShouldNotBeNull();
            expense.Amount.ShouldBe(amount);
        }
        
        private async Task<ExpenseDTO> AddExpense(string categoryName, string description = null, decimal amount = 100M, Currency currency = Currency.PLN)
        {
            _testFixture.NavigateTo("/");
            var home = _testFixture.AppComponent.FindComponent<Home>();
            _homePage = new HomePageObject(home);
            _testFixture.AppComponent.WaitForAssertion(() =>
            {
                _homePage.HasExpensesTable.ShouldBeTrue();
            });
            description ??= $"Description-{Guid.NewGuid()}";
            var modal = _homePage.ClickAddExpense(_testFixture.AppComponent);
            var service = _testFixture.TestContext.Services.GetRequiredService<IObservableExpenseService>();
            var addSubscription = service.WaitForAddAsync();
            var getAllSubscription = service.WaitForGetAllAsync();
            modal.EnterDescription(description)
                 .EnterAmount(amount)
                 .SelectCurrencyByName(currency.ToString())
                 .SelectCategoryByCategoryName(categoryName);
            await modal.ClickSubmit();
            await addSubscription;
            await getAllSubscription;
            _testFixture.AppComponent.Render();
            var expense = _homePage.GetExpenseByDescription(description);
            expense.ShouldNotBeNull();
            return new ExpenseDTO
            {
                Id = expense.Id,
                Amount = expense.Amount,
                CategoryId = expense.CategoryId,
                Currency = expense.Currency,
                Description = expense.Description,
                Rate = expense.Rate
            };
        }

        private async Task<CategoryDTO> AddCategory(string name = null, decimal budget = 1000M)
        {
            name ??= $"Category-{Guid.NewGuid()}";
            _testFixture.NavigateTo("/categories");
            var categories = _testFixture.AppComponent.FindComponent<Categories>();
            _categoriesPage = new CategoriesPageObject(categories);
            _testFixture.AppComponent.WaitForAssertion(() =>
            {
                _categoriesPage.HasCategoriesTable.ShouldBeTrue();
            });
            var addCategoryModal = _categoriesPage.ClickAddCategory(_testFixture.AppComponent);
            var service = _testFixture.TestContext.Services.GetRequiredService<IObservableCategoryService>();
            var addSubscription = service.WaitForAddAsync();
            var getAllSubscription = service.WaitForGetAllAsync();
            await addCategoryModal.EnterName(name)
                                  .EnterBudget(budget)
                                  .ClickSubmit();
            await addSubscription;
            await getAllSubscription;
            _testFixture.AppComponent.Render();
            var category = _categoriesPage.GetCategoryByName(name);
            category.ShouldNotBeNull();
            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Budget = category.Budget
            };
        }
    }
}
