using Bunit;
using ExpenseTracker.IntegrationTests.Setup;
using ExpenseTracker.UI.Authentication;
using ExpenseTracker.UI.IntegrationTests.PageObjects;
using ExpenseTracker.UI.Pages;
using Microsoft.Extensions.DependencyInjection;
using ExpenseTracker.IntegrationTests.Setup.Services;
using Shouldly;
using System.Security.Claims;

namespace ExpenseTracker.UI.IntegrationTests.Pages
{
    public class CategoriesPageTests
    {
        private readonly TestFixture _testFixture = new();
        private CategoriesPageObject _categoriesPage;

        public CategoriesPageTests()
        {
            _testFixture.TestContext.AuthContext.SetAuthorized("testowyUser");
            _testFixture.TestContext.AuthContext.SetClaims(new Claim(AuthConstants.ID_CLAIM_TYPE, "1"));
            _testFixture.NavigateTo("/categories", true);
            var categories = _testFixture.AppComponent.FindComponent<Categories>();
            _categoriesPage = new CategoriesPageObject(categories);
            _testFixture.AppComponent.WaitForAssertion(() =>
            {
                _categoriesPage.HasCategoriesTable.ShouldBeTrue();
            });
        }

        [Fact]
        public void CategoriesPage_WithCategories_ShouldRenderTable()
        {
            // Arrange Act
            var count = _categoriesPage.CategoriesCount;

            // Assert
            _categoriesPage.HasCategoriesTable.ShouldBeTrue();
            count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void CategoriesPage_SearchCategory_ShouldFilterRows()
        {
            // Arrange Act
            _categoriesPage.SearchCategory("Szkolenie");

            // Assert
            _categoriesPage.GetCategoryByName("Szkolenie C#").ShouldNotBeNull();
        }

        [Fact]
        public void CategoriesPage_EditCategoryRow_ShouldShowModalWithEditForm()
        {
            // Arrange
            var row = _categoriesPage.GetCategoryByName("Sprzęt komputerowy");

            // Act
            var modal = row!.Edit(_testFixture.AppComponent);

            // Assert
            modal.Name.ShouldBe("Sprzęt komputerowy");
        }

        [Fact]
        public void CategoriesPage_DeleteCategoryRow_ShouldShowConfirmationModal()
        {
            // Arrange
            var row = _categoriesPage.GetCategoryByName("Sprzęt komputerowy");

            // Act
            var modal = row!.Delete(_testFixture.AppComponent);

            // Assert
            modal.Message!.ShouldContain("Czy chcesz usunąć kategorię?");
        }

        [Fact]
        public void CategoriesPage_AddCategoryButton_ShouldShowModalWithAddForm()
        {
            // Arrange Act
            var modal = _categoriesPage.ClickAddCategory(_testFixture.AppComponent);

            // Assert
            modal.Title!.ShouldContain("Dodaj kategorię");
        }

        [Theory]
        [InlineData("", 10, 1)]
        [InlineData("name123", -10, 1)]
        [InlineData("", -10, 2)]
        public async Task CategoriesPage_AddCategoryButtonAndFillFormWithInvalidData_ShouldShowValidationErrors(string name, decimal budget, int expectedErrorsCount)
        {
            // Arrange
            var modal = _categoriesPage.ClickAddCategory(_testFixture.AppComponent);

            // Act
            await modal.EnterName(name)
                       .EnterBudget(budget)
                       .ClickSubmit();

            // Assert
            modal.FormErrors.ShouldNotBeEmpty();
            modal.FormErrors.Count().ShouldBe(expectedErrorsCount);
        }

        [Fact]
        public async Task CategoriesPage_AddCategoryButtonAndFillForm_ShouldAddNewCategory()
        {
            // Arrange
            var name = "Budget#2025";
            var budget = 2025M;
            var modal = _categoriesPage.ClickAddCategory(_testFixture.AppComponent);
            var service = _testFixture.TestContext.Services.GetRequiredService<IObservableCategoryService>();
            var addSubscription = service.WaitForAddAsync();
            var getAllSubscription = service.WaitForGetAllAsync();

            // Act
            await modal.EnterName(name)
                       .EnterBudget(budget)
                       .ClickSubmit();

            // Assert
            await addSubscription;
            await getAllSubscription;
            _testFixture.AppComponent.Render();
            var category = _categoriesPage.GetCategoryByName(name);
            category.ShouldNotBeNull();
            category.Name.ShouldBe(name);
            category.Budget.ShouldBe(budget);
        }

        [Fact]
        public async Task CategoriesPage_EditCategoryFillForm_ShouldUpdateCategory()
        {
            // Arrange
            var category = await AddCategory($"Name_{Guid.NewGuid()}", 2025M);
            var service = _testFixture.TestContext.Services.GetRequiredService<IObservableCategoryService>();
            var modal = category.Edit(_testFixture.AppComponent);
            var updateSubscription = service.WaitForUpdateAsync();
            var getAllSubscription = service.WaitForGetAllAsync();
            var newName = category.Name + "_modified";
            var newBudget = category.Budget + 9995M;

            // Act
            await modal.EnterName(newName)
                       .EnterBudget(newBudget)
                       .ClickSubmit();

            // Assert
            await updateSubscription;
            await getAllSubscription;
            _testFixture.AppComponent.Render();
            var categoryUpdated = _categoriesPage.GetCategoryByName(newName);
            categoryUpdated.ShouldNotBeNull();
            categoryUpdated.Name.ShouldBe(newName);
            categoryUpdated.Budget.ShouldBe(newBudget);
        }

        [Fact]
        public async Task CategoriesPage_DeleteCategoryClickYes_ShouldDeleteCategory()
        {
            // Arrange
            var category = await AddCategory($"Name_{Guid.NewGuid()}", 20202025M);
            var deleteModal = category.Delete(_testFixture.AppComponent);
            var service = _testFixture.TestContext.Services.GetRequiredService<IObservableCategoryService>();
            var deleteSubscription = service.WaitForDeleteAsync();
            var getAllSubscription = service.WaitForGetAllAsync();

            // Act
            await deleteModal.ClickYes();

            // Assert
            await deleteSubscription;
            await getAllSubscription;
            _testFixture.AppComponent.Render();
            var categoryDeleted = _categoriesPage.GetCategoryByName(category.Name);
            categoryDeleted.ShouldBeNull();
        }

        private async Task<CategoryPageRowDTO> AddCategory(string name, decimal budget)
        {
            var modal = _categoriesPage.ClickAddCategory(_testFixture.AppComponent);
            var service = _testFixture.TestContext.Services.GetRequiredService<IObservableCategoryService>();
            var addSubscription = service.WaitForAddAsync();
            var getAllSubscription = service.WaitForGetAllAsync();
            await modal.EnterName(name)
                       .EnterBudget(budget)
                       .ClickSubmit();
            await addSubscription;
            await getAllSubscription;
            _testFixture.AppComponent.Render();
            return _categoriesPage.GetCategoryByName(name)!;
        }
    }
}
