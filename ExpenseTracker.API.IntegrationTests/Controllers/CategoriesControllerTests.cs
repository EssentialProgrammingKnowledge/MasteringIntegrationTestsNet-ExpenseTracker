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

namespace ExpenseTracker.API.IntegrationTests.Controllers
{
    public class CategoriesControllerTests : BaseTestCollectionFixture
    {
        private const string URL = "/api/categories";
        private readonly CategoryTestService _categoryTestService;
        private readonly ExpenseContext _expenseContext;
        private readonly int _userId = 1;

        public CategoriesControllerTests(WebAppFactoryFixture webAppFactoryFixture, ITestOutputHelper testOutputHelper)
            : base(webAppFactoryFixture, testOutputHelper)
        {
            UserAccessor.SetUser(TestUser.Admin(_userId.ToString()).SetEmail("admin@admin.com").SetUserId(Guid.NewGuid().ToString()));
            _categoryTestService = webAppFactoryFixture.Services.GetRequiredService<CategoryTestService>();
            _expenseContext = webAppFactoryFixture.Services.GetRequiredService<ExpenseContext>();
        }

        [Fact]
        public async Task Categories_GetAllCategories_ShouldReturnOnlyCategoriesForCurrentUser()
        {
            // Arrange Act
            var response = await Client.GetAsync(URL);

            // Assert
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<List<CategoryDTO>>();
            result.ShouldNotBeNull().ShouldNotBeEmpty();
            var categoriesCount = await _expenseContext.Categories.Where(c => c.UserId == _userId).AsNoTracking().CountAsync();
            result.Count().ShouldBe(categoriesCount);
        }


        [Fact]
        public async Task Categories_AddValidCategory_ShouldCreateCategory()
        {
            // Arrange
            var dto = new CategoryDTO(0, "FirstCategory", 199M);

            // Act
            var response = await Client.PostAsJsonAsync(URL, dto);

            // Assert
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<CategoryDTO>();
            result.ShouldNotBeNull();
            result.Name.ShouldBe(dto.Name);
            result.Budget.ShouldBe(dto.Budget);
            response.Headers.ShouldContain(h => h.Key == HeaderNames.Location && h.Value.Any(v => v.Contains($"/api/categories/{result.Id}", StringComparison.InvariantCultureIgnoreCase)));
        }

        [Fact]
        public async Task Categories_EditValidCategory_ShouldUpdateCategory()
        {
            // Arrange
            var category = await _categoryTestService.AddCategory();
            var categoryToUpdate = new CategoryDTO(category.Id, "NewCategory", 250M);

            // Act
            var response = await Client.PutAsJsonAsync($"{URL}/{category.Id}", categoryToUpdate);

            // Assert
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var categoryUpdated = await response.Content.ReadFromJsonAsync<CategoryDTO>();
            categoryUpdated.ShouldNotBeNull();
            categoryUpdated.Name.ShouldBe(categoryToUpdate.Name);
            categoryUpdated.Budget.ShouldBe(categoryToUpdate.Budget);
        }

        [Fact]
        public async Task Categories_EditInvalidCategoryId_ShouldReturnNotFound()
        {
            // Arrange
            var category = new CategoryDTO(100000, "AAA", 250M);

            // Act
            var response = await Client.PutAsJsonAsync($"{URL}/{category.Id}", category);

            // Assert
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Categories_DeleteValidId_ShouldDeleteCategory()
        {
            // Arrange
            var category = await _categoryTestService.AddCategory();

            // Act
            var response = await Client.DeleteAsync($"{URL}/{category.Id}");

            // Assert
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
            var categoryDeleted = await _expenseContext.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == category.Id);
            categoryDeleted.ShouldBeNull();
        }

        [Fact]
        public async Task Categories_DeleteInvalidId_ShouldReturnNotFound()
        {
            // Arrange Act
            var response = await Client.DeleteAsync($"{URL}/1000000");

            // Assert
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}
