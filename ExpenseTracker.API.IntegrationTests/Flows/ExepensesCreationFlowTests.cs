using ExpenseTracker.API.Database;
using ExpenseTracker.API.DTO;
using ExpenseTracker.API.IntegrationTests.Setup.Auth;
using ExpenseTracker.API.IntegrationTests.Setup.BaseTests.CollectionFixtures;
using ExpenseTracker.API.IntegrationTests.Setup.Hosting;
using ExpenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace ExpenseTracker.API.IntegrationTests.Flows
{
    public class ExepensesCreationFlowTests : BaseTestCollectionFixture
    {
        private const string CATEGORIES_URL = "/api/categories";
        private const string EXPENSES_URL = "/api/expenses";
        private readonly ExpenseContext _expenseContext;
        private readonly decimal chfRate = 4M;
        private readonly decimal usdRate = 2M;
        private readonly decimal euroRate = 2.5M;

        public ExepensesCreationFlowTests(WebAppFactoryFixture webAppFactoryFixture, ITestOutputHelper testOutputHelper)
            : base(webAppFactoryFixture, testOutputHelper)
        {
            UserAccessor.SetUser(TestUser.Admin("1").SetEmail("admin@admin.com").SetUserId(Guid.NewGuid().ToString()));
            _expenseContext = webAppFactoryFixture.Services.GetRequiredService<ExpenseContext>();
            webAppFactoryFixture.SetupNbpRateService(mock =>
            {
                mock.Setup(m => m.GetRatesAsync()).ReturnsAsync(
                [
                    new ()
                    {
                        Code = Currency.USD.ToString(),
                        Currency = Currency.USD.ToString(),
                        Mid = usdRate,
                        EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow)
                    },
                    new ()
                    {
                        Code = Currency.EUR.ToString(),
                        Currency = Currency.EUR.ToString(),
                        Mid = euroRate,
                        EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow)
                    },
                    new ()
                    {
                        Code = Currency.CHF.ToString(),
                        Currency = Currency.CHF.ToString(),
                        Mid = chfRate,
                        EffectiveDate = DateOnly.FromDateTime(DateTime.UtcNow)
                    }
                ]);
            });
        }

        [Fact]
        public async Task ExpensesCreation_BudgetNotExceeded_ShouldAddAllExpenses()
        {
            // Arrange
            var tripDto = new CategoryDTO(0, "Wycieczka", 5500M);

            // Act
            var responseAddCategory = await Client.PostAsJsonAsync(CATEGORIES_URL, tripDto);

            // Assert
            responseAddCategory.ShouldNotBeNull();
            responseAddCategory.StatusCode.ShouldBe(HttpStatusCode.Created);
            var tripCategory = await responseAddCategory.Content.ReadFromJsonAsync<CategoryDTO>();
            tripCategory.ShouldNotBeNull();
            tripCategory.Name.ShouldBe(tripDto.Name);
            tripCategory.Budget.ShouldBe(tripDto.Budget);

            // Arrange
            var bookingDto = new ExpenseDTO(0, 800M, tripCategory.Id, "Booking", Currency.CHF, decimal.One);

            // Act
            var bookingResponse = await Client.PostAsJsonAsync(EXPENSES_URL, bookingDto);

            // Assert
            bookingResponse.ShouldNotBeNull();
            bookingResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
            var bookingExpense = await bookingResponse.Content.ReadFromJsonAsync<ExpenseDetailsDTO>();
            bookingExpense.ShouldNotBeNull();
            bookingExpense.Description.ShouldBe(bookingDto.Description);
            bookingExpense.Category.Id.ShouldBe(bookingDto.CategoryId);
            bookingExpense.Amount.ShouldBe(bookingDto.Amount);
            bookingExpense.Currency.ShouldBe(bookingDto.Currency);
            (bookingExpense.Amount * bookingExpense.Rate).ShouldBe(bookingDto.Amount * chfRate);
            bookingExpense.Rate.ShouldBeGreaterThan(decimal.One);

            // Arrange
            var rentCarDto = new ExpenseDTO(0, 200M, tripCategory.Id, "Rent Car", Currency.USD, decimal.One);

            // Act
            var rentCarResponse = await Client.PostAsJsonAsync(EXPENSES_URL, rentCarDto);

            // Assert
            rentCarResponse.ShouldNotBeNull();
            rentCarResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
            var rentCarExpense = await rentCarResponse.Content.ReadFromJsonAsync<ExpenseDetailsDTO>();
            rentCarExpense.ShouldNotBeNull();
            rentCarExpense.Description.ShouldBe(rentCarDto.Description);
            rentCarExpense.Category.Id.ShouldBe(rentCarDto.CategoryId);
            rentCarExpense.Amount.ShouldBe(rentCarDto.Amount);
            rentCarExpense.Currency.ShouldBe(rentCarDto.Currency);
            (rentCarExpense.Amount * rentCarExpense.Rate).ShouldBe(rentCarDto.Amount * usdRate);
            rentCarExpense.Rate.ShouldBeGreaterThan(decimal.One);

            // Arrange
            var eatingDto = new ExpenseDTO(0, 200M, tripCategory.Id, "Jedzenie", Currency.EUR, decimal.One);

            // Act
            var eatingResponse = await Client.PostAsJsonAsync(EXPENSES_URL, eatingDto);

            // Assert
            eatingResponse.ShouldNotBeNull();
            eatingResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
            var eatingExpense = await eatingResponse.Content.ReadFromJsonAsync<ExpenseDetailsDTO>();
            eatingExpense.ShouldNotBeNull();
            eatingExpense.Description.ShouldBe(eatingDto.Description);
            eatingExpense.Category.Id.ShouldBe(eatingDto.CategoryId);
            eatingExpense.Amount.ShouldBe(eatingDto.Amount);
            eatingExpense.Currency.ShouldBe(eatingDto.Currency);
            (eatingExpense.Amount * eatingExpense.Rate).ShouldBe(eatingDto.Amount * euroRate);
            eatingExpense.Rate.ShouldBeGreaterThan(decimal.One);

            // Arrange
            var trainTicketDto = new ExpenseDTO(0, 200M, tripCategory.Id, "Bilety na pociąg", Currency.PLN, decimal.One);

            // Act
            var trainTicketResponse = await Client.PostAsJsonAsync(EXPENSES_URL, trainTicketDto);

            // Assert
            trainTicketResponse.ShouldNotBeNull();
            trainTicketResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
            var trainTicketExpense = await trainTicketResponse.Content.ReadFromJsonAsync<ExpenseDetailsDTO>();
            trainTicketExpense.ShouldNotBeNull();
            trainTicketExpense.Description.ShouldBe(trainTicketDto.Description);
            trainTicketExpense.Category.Id.ShouldBe(trainTicketDto.CategoryId);
            trainTicketExpense.Amount.ShouldBe(trainTicketDto.Amount);
            trainTicketExpense.Currency.ShouldBe(trainTicketDto.Currency);
            trainTicketExpense.Amount.ShouldBe(trainTicketDto.Amount);
            trainTicketExpense.Rate.ShouldBe(decimal.One);

            // Act
            var categoryUpdatedResponse = await _expenseContext.Categories.Include(e => e.Expenses)
                                                               .FirstOrDefaultAsync(c => c.Id == tripCategory.Id);

            // Assert
            categoryUpdatedResponse.ShouldNotBeNull();
            var tripExpenses = categoryUpdatedResponse.Expenses.Sum(e => e.GetTotalAmount());
            tripExpenses.ShouldBeLessThan(tripCategory.Budget);
        }

        [Fact]
        public async Task ExpensesCreation_BudgetExceeded_ShouldReturnBadRequestAndNotAddAllExpenses()
        {
            // Arrange
            var homeExpnesesDto = new CategoryDTO(0, "Wydatki domowe", 1000M);

            // Act
            var responseAddCategory = await Client.PostAsJsonAsync(CATEGORIES_URL, homeExpnesesDto);

            // Assert
            responseAddCategory.ShouldNotBeNull();
            responseAddCategory.StatusCode.ShouldBe(HttpStatusCode.Created);
            var homeExpenseCategory = await responseAddCategory.Content.ReadFromJsonAsync<CategoryDTO>();
            homeExpenseCategory.ShouldNotBeNull();
            homeExpenseCategory.Name.ShouldBe(homeExpnesesDto.Name);
            homeExpenseCategory.Budget.ShouldBe(homeExpnesesDto.Budget);

            // Arrange
            var headphonseDto = new ExpenseDTO(0, 100M, homeExpenseCategory.Id, "Słuchawki", Currency.CHF, decimal.One);

            // Act
            var headphonesResponse = await Client.PostAsJsonAsync(EXPENSES_URL, headphonseDto);

            // Assert
            headphonesResponse.ShouldNotBeNull();
            headphonesResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
            var headphonesExpense = await headphonesResponse.Content.ReadFromJsonAsync<ExpenseDetailsDTO>();
            headphonesExpense.ShouldNotBeNull();
            headphonesExpense.Description.ShouldBe(headphonseDto.Description);
            headphonesExpense.Category.Id.ShouldBe(headphonseDto.CategoryId);
            headphonesExpense.Amount.ShouldBe(headphonseDto.Amount);
            headphonesExpense.Currency.ShouldBe(headphonseDto.Currency);
            (headphonesExpense.Amount * headphonesExpense.Rate).ShouldBe(headphonseDto.Amount * chfRate);
            headphonesExpense.Rate.ShouldBeGreaterThan(decimal.One);

            // Arrange
            var gameDto = new ExpenseDTO(0, 300M, homeExpenseCategory.Id, "GTA 6", Currency.USD, decimal.One);

            // Act
            var gameResponse = await Client.PostAsJsonAsync(EXPENSES_URL, gameDto);

            // Assert
            gameResponse.ShouldNotBeNull();
            gameResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
            var gameExpense = await gameResponse.Content.ReadFromJsonAsync<ExpenseDetailsDTO>();
            gameExpense.ShouldNotBeNull();
            gameExpense.Description.ShouldBe(gameDto.Description);
            gameExpense.Category.Id.ShouldBe(gameDto.CategoryId);
            gameExpense.Amount.ShouldBe(gameDto.Amount);
            gameExpense.Currency.ShouldBe(gameDto.Currency);
            (gameExpense.Amount * gameExpense.Rate).ShouldBe(gameDto.Amount * usdRate);
            gameExpense.Rate.ShouldBeGreaterThan(decimal.One);

            // Arrange
            var phoneDto = new ExpenseDTO(0, 800M, homeExpenseCategory.Id, "Telefon", Currency.EUR, decimal.One);

            // Act
            var phoneResponse = await Client.PostAsJsonAsync(EXPENSES_URL, headphonseDto);

            // Assert
            phoneResponse.ShouldNotBeNull();
            phoneResponse.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var result = await phoneResponse.Content.ReadFromJsonAsync<ErrorMessage>();
            result.ShouldNotBeNull();
            result.Code.ShouldBe("EXPENSE_AMOUNT_EXCEEDS_BUDGET");

            // Act
            var categoryUpdatedResponse = await _expenseContext.Categories.Include(e => e.Expenses)
                                                               .FirstOrDefaultAsync(c => c.Id == homeExpenseCategory.Id);

            // Assert
            categoryUpdatedResponse.ShouldNotBeNull();
            var tripExpenses = categoryUpdatedResponse.Expenses.Sum(e => e.GetTotalAmount());
            tripExpenses.ShouldBeLessThanOrEqualTo(homeExpenseCategory.Budget);
        }
    }
}
