using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using ExpenseTracker.API.Database;
using ExpenseTracker.API.DTO;
using ExpenseTracker.API.IntegrationTests.Setup.Auth;
using ExpenseTracker.API.IntegrationTests.Setup.BaseTests.CollectionFixtures;
using ExpenseTracker.API.IntegrationTests.Setup.Hosting;
using ExpenseTracker.API.IntegrationTests.TestServices;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;
using ExpenseTracker.API.Models;
using Moq;

namespace ExpenseTracker.API.IntegrationTests.Controllers
{
    public class ExpensesControllerTests : BaseTestCollectionFixture, IAsyncLifetime
    {
        private const string URL = "/api/expenses";
        private readonly WebAppFactoryFixture _webAppFactoryFixture;
        private readonly CategoryTestService _categoryTestService;
        private readonly ExpenseTestService _expenseTestService;
        private readonly ExpenseContext _expenseContext;
        private CategoryDTO category;

        public ExpensesControllerTests(WebAppFactoryFixture webAppFactoryFixture, ITestOutputHelper testOutputHelper)
            : base(webAppFactoryFixture, testOutputHelper)
        {
            UserAccessor.SetUser(TestUser.Admin("2").SetEmail("admin@admin.com").SetUserId(Guid.NewGuid().ToString()));
            _webAppFactoryFixture = webAppFactoryFixture;
            _categoryTestService = webAppFactoryFixture.Services.GetRequiredService<CategoryTestService>();
            _expenseTestService = webAppFactoryFixture.Services.GetRequiredService<ExpenseTestService>();
            _expenseContext = webAppFactoryFixture.Services.GetRequiredService<ExpenseContext>();
        }

        public async Task InitializeAsync()
        {
            category = await _categoryTestService.AddCategory($"{Guid.NewGuid()}", 1000M);
            _webAppFactoryFixture.SetupNbpRateService(mock =>
            {
                mock.Setup(m => m.GetRatesAsync()).ReturnsAsync(
                [
                    new ()
                    {
                        Code = Currency.USD.ToString(),
                        Currency = Currency.USD.ToString(),
                        Mid = 2M,
                        EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow)
                    },
                    new ()
                    {
                        Code = Currency.EUR.ToString(),
                        Currency = Currency.EUR.ToString(),
                        Mid = 2M,
                        EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow)
                    },
                    new ()
                    {
                        Code = Currency.CHF.ToString(),
                        Currency = Currency.CHF.ToString(),
                        Mid = 4M,
                        EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow)
                    }
                ]);
            });
        }

        public Task DisposeAsync()
        {
            _webAppFactoryFixture.SetupNbpRateService(action => action.Reset());
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Expenses_AddValidExpense_ShouldCreateExpense()
        {
            // Arrange
            var dto = new ExpenseDTO(0, 150M, category.Id, "FirstExpense", Currency.CHF, decimal.One);

            // Act
            var response = await Client.PostAsJsonAsync(URL, dto);

            // Assert
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<ExpenseDetailsDTO>();
            result.ShouldNotBeNull();
            result.Description.ShouldBe(dto.Description);
            result.Category.Id.ShouldBe(dto.CategoryId);
            result.Amount.ShouldBe(dto.Amount);
            result.Currency.ShouldBe(dto.Currency);
            result.Rate.ShouldBeGreaterThan(decimal.One);
            response.Headers.ShouldContain(h => h.Key == HeaderNames.Location && h.Value.Any(v => v.Contains($"/api/expenses/{result.Id}", StringComparison.InvariantCultureIgnoreCase)));
        }

        [Fact]
        public async Task Expenses_EditValidExpense_ShouldUpdateExpense()
        {
            // Arrange
            var expense = await _expenseTestService.AddExpense(category.Id);
            var expenseToUpdate = new ExpenseDTO(expense.Id, 150M, expense.Category.Id, expense.Description + Guid.NewGuid().ToString(), Currency.USD, 2M);

            // Act
            var response = await Client.PutAsJsonAsync($"{URL}/{expense.Id}", expenseToUpdate);

            // Assert
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var expenseUpdated = await response.Content.ReadFromJsonAsync<ExpenseDetailsDTO>();
            expenseUpdated.ShouldNotBeNull();
            expenseUpdated.Description.ShouldBe(expenseToUpdate.Description);
            expenseUpdated.Category.Id.ShouldBe(expense.Category.Id);
            expenseUpdated.Amount.ShouldBe(expenseToUpdate.Amount);
            expenseUpdated.Currency.ShouldBe(expense.Currency);
            expenseUpdated.Currency.ShouldNotBe(expenseToUpdate.Currency);
            expenseUpdated.Rate.ShouldBe(expense.Rate);
            expenseUpdated.Rate.ShouldNotBe(expenseToUpdate.Rate);
        }

        [Fact]
        public async Task Expenses_EditInvalidExpenseId_ShouldReturnNotFound()
        {
            // Arrange
            var expense = new ExpenseDTO(100000, 100M, 1, "AAA", Currency.EUR, 4M);

            // Act
            var response = await Client.PutAsJsonAsync($"{URL}/{expense.Id}", expense);

            // Assert
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Expenses_DeleteValidId_ShouldDeleteExpense()
        {
            // Arrange
            var expense = await _expenseTestService.AddExpense(category.Id);

            // Act
            var response = await Client.DeleteAsync($"{URL}/{expense.Id}");

            // Assert
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
            var categoryDeleted = await _expenseContext.Expenses.AsNoTracking().FirstOrDefaultAsync(c => c.Id == expense.Id);
            categoryDeleted.ShouldBeNull();
        }

        [Fact]
        public async Task Expenses_DeleteInvalidId_ShouldReturnNotFound()
        {
            // Arrange Act
            var response = await Client.DeleteAsync($"{URL}/1000000");

            // Assert
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}
