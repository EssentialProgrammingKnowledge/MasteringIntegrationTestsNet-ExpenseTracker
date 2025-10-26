using Bunit;
using ExpenseTracker.IntegrationTests.Setup;
using ExpenseTracker.UI.Authentication;
using ExpenseTracker.UI.IntegrationTests.PageObjects;
using ExpenseTracker.UI.Models;
using ExpenseTracker.UI.Pages;
using ExpenseTracker.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using ExpenseTracker.IntegrationTests.Setup.Services;
using Shouldly;
using System.Security.Claims;

namespace ExpenseTracker.UI.IntegrationTests.Pages
{
    public class HomePageTests
    {
        private readonly TestFixture _testFixture = new();
        private HomePageObject _homePage;

        public HomePageTests()
        {
            _testFixture.TestContext.AuthContext.SetAuthorized("testowyUser");
            _testFixture.TestContext.AuthContext.SetClaims(new Claim(AuthConstants.ID_CLAIM_TYPE, "2"));
            _testFixture.NavigateTo("/", true);
            var home = _testFixture.AppComponent.FindComponent<Home>();
            _homePage = new HomePageObject(home);
            _testFixture.AppComponent.WaitForAssertion(() =>
            {
                _homePage.HasExpensesTable.ShouldBeTrue();
            }, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void HomePage_WithExpenses_ShouldRenderTable()
        {
            // Arrange Act
            var count = _homePage.ExpensesCount;

            // Assert
            _homePage.HasExpensesTable.ShouldBeTrue();
            count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void HomePage_SearchExpense_ShouldFilterRows()
        {
            // Arrange Act
            _homePage.SearchExpense("Laptop");

            // Assert
            _homePage.GetExpenseByDescription("Laptop Dell").ShouldNotBeNull();
        }

        [Fact]
        public void HomePage_EditExpenseRow_ShouldShowModalWithEditForm()
        {
            // Arrange
            var row = _homePage.GetExpenseByDescription("Laptop Dell");

            // Act
            var modal = row!.Edit(_testFixture.AppComponent);

            // Assert
            modal.Description.ShouldBe("Laptop Dell");
        }

        [Fact]
        public void HomePage_DeleteExpenseRow_ShouldShowConfirmationModal()
        {
            // Arrange
            var row = _homePage.GetExpenseByDescription("Laptop Dell");

            // Act
            var modal = row!.Delete(_testFixture.AppComponent);

            // Assert
            modal.Message!.ShouldContain("Czy chcesz usunąć wydatek?");
        }

        [Fact]
        public void HomePage_AddExpenseButton_ShouldShowModalWithAddForm()
        {
            // Arrange Act
            var modal = _homePage.ClickAddExpense(_testFixture.AppComponent);

            // Assert
            modal.Title!.ShouldContain("Dodaj wydatek");
        }

        [Theory]
        [InlineData("", 10, 1)]
        [InlineData("name123", -10, 1)]
        [InlineData("", -10, 2)]
        public async Task HomePage_AddExpenseButtonAndFillFormWithInvalidData_ShouldShowValidationErrors(string description, decimal amount, int expectedErrorsCount)
        {
            // Arrange
            var modal = _homePage.ClickAddExpense(_testFixture.AppComponent);

            // Act
            await modal.EnterDescription(description)
                       .EnterAmount(amount)
                       .SelectCurrency(1)
                       .SelectCategory(1)
                       .ClickSubmit();

            // Assert
            modal.FormErrors.ShouldNotBeEmpty();
            modal.FormErrors.Count().ShouldBe(expectedErrorsCount);
        }

        [Fact]
        public async Task HomePage_AddExpenseButtonAndFillForm_ShouldAddNewExpense()
        {
            // Arrange
            var description = "Description#2025";
            var amount = 2025M;
            var currency = Currency.USD;
            var categoryName = "Szkolenie C#";
            var modal = _homePage.ClickAddExpense(_testFixture.AppComponent);
            var categoryService = _testFixture.TestContext.Services.GetRequiredService<ICategoryService>();
            var service = _testFixture.TestContext.Services.GetRequiredService<IObservableExpenseService>();
            var addSubscription = service.WaitForAddAsync();
            var getAllSubscription = service.WaitForGetAllAsync();
            var category = (await categoryService.GetAll()).Data!.First(c => c.Name == categoryName);

            // Act
            modal.EnterDescription(description)
                 .EnterAmount(amount)
                 .SelectCurrencyByName(currency.ToString())
                 .SelectCategoryByCategoryName(categoryName);

            // Assert
            modal.HasRate.ShouldBeTrue();
            modal.TotalAmount.ShouldNotBeNullOrWhiteSpace();

            // Act
            await modal.ClickSubmit();

            // Assert
            await addSubscription;
            await getAllSubscription;
            _testFixture.AppComponent.Render();
            var expense = _homePage.GetExpenseByDescription(description);
            expense.ShouldNotBeNull();
            expense.Description.ShouldBe(description);
            expense.Amount.ShouldBe(amount);
            expense.Currency.ShouldBe(currency);
            expense.CategoryId.ShouldBeGreaterThan(0);
            expense.CategoryId.ShouldBe(category.Id);
        }

        [Fact]
        public async Task HomePage_EditExpenseFillForm_ShouldUpdateExpense()
        {
            // Arrange
            var category = await AddExpense($"Name_{Guid.NewGuid()}", 2025M, "Sprzęt komputerowy", Currency.CHF);
            var service = _testFixture.TestContext.Services.GetRequiredService<IObservableExpenseService>();
            var modal = category.Edit(_testFixture.AppComponent);
            var updateSubscription = service.WaitForUpdateAsync();
            var getAllSubscription = service.WaitForGetAllAsync();
            var newDescription = category.Description + "_modified";
            var newAmount = category.Amount + 9995M;
            modal.IsCurrencyDisabled.ShouldBeTrue();
            modal.HasExistingRate.ShouldBeTrue();
            modal.Rate.ShouldNotBeNullOrWhiteSpace();

            // Act
            await modal.EnterDescription(newDescription)
                       .EnterAmount(newAmount)
                       .ClickSubmit();

            // Assert
            await updateSubscription;
            await getAllSubscription;
            _testFixture.AppComponent.Render();
            var expenseUpdated = _homePage.GetExpenseByDescription(newDescription);
            expenseUpdated.ShouldNotBeNull();
            expenseUpdated.Description.ShouldBe(newDescription);
            expenseUpdated.Amount.ShouldBe(newAmount);
        }

        [Fact]
        public async Task HomePage_DeleteExpenseClickYes_ShouldDeleteExpense()
        {
            // Arrange
            var expense = await AddExpense($"Name_{Guid.NewGuid()}", 20202025M, "Szkolenie C#", Currency.EUR);
            var deleteModal = expense.Delete(_testFixture.AppComponent);
            var service = _testFixture.TestContext.Services.GetRequiredService<IObservableExpenseService>();
            var deleteSubscription = service.WaitForDeleteAsync();
            var getAllSubscription = service.WaitForGetAllAsync();

            // Act
            await deleteModal.ClickYes();

            // Assert
            await deleteSubscription;
            await getAllSubscription;
            _testFixture.AppComponent.Render();
            var expenseDeleted = _homePage.GetExpenseByDescription(expense.Description);
            expenseDeleted.ShouldBeNull();
        }

        private async Task<ExpensePageRowDTO> AddExpense(string desciption, decimal amount, string categoryName, Currency currency = Currency.PLN)
        {
            var modal = _homePage.ClickAddExpense(_testFixture.AppComponent);
            var service = _testFixture.TestContext.Services.GetRequiredService<IObservableExpenseService>();
            var addSubscription = service.WaitForAddAsync();
            var getAllSubscription = service.WaitForGetAllAsync();
            await modal.EnterDescription(desciption)
                       .EnterAmount(amount)
                       .SelectCategoryByCategoryName(categoryName)
                       .SelectCurrencyByName(currency.ToString())
                       .ClickSubmit();
            await addSubscription;
            await getAllSubscription;
            _testFixture.AppComponent.Render();
            return _homePage.GetExpenseByDescription(desciption)!;
        }
    }
}
