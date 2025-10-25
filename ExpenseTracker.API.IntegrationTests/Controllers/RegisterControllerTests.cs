using ExpenseTracker.API.Database;
using ExpenseTracker.API.DTO;
using ExpenseTracker.API.IntegrationTests.Setup.Auth;
using ExpenseTracker.API.IntegrationTests.Setup.BaseTests.CollectionFixtures;
using ExpenseTracker.API.IntegrationTests.Setup.Hosting;
using ExpenseTracker.API.Security;
using ExpenseTracker.API.Validations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace ExpenseTracker.API.IntegrationTests.Controllers
{
    public class RegisterControllerTests : BaseTestCollectionFixture
    {
        private const string URL = "/api/register";
        private readonly ITokenParser _tokenParser;
        private readonly ExpenseContext _dbContext;

        public RegisterControllerTests(WebAppFactoryFixture webAppFactoryFixture, ITestOutputHelper testOutputHelper)
            : base(webAppFactoryFixture, testOutputHelper)
        {
            _tokenParser = webAppFactoryFixture.Services.GetRequiredService<ITokenParser>();
            _dbContext = webAppFactoryFixture.Services.GetRequiredService<ExpenseContext>();
        }

        [Fact]
        public async Task Register_CreateNewAccountValidCredentials_ShouldAddNewUserAndReturnToken()
        {
            // Arrange
            var dto = new RegisterDTO("FirstNameTest", "LastNameTest", $"email{Guid.NewGuid():N}@email.com", "P@AsW0Rd!123!321");

            // Act
            var response = await Client.PostAsJsonAsync(URL, dto);

            // Assert
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<RegisterResultDTO>();
            result.ShouldNotBeNull();
            result.AccessToken.ShouldNotBeNull();
            var userClaims = _tokenParser.GetUserClaims(result.AccessToken);
            userClaims.ShouldNotBeNull().ShouldNotBeEmpty();
            userClaims.ShouldContain(c => c.Type == JwtClaimConstants.NAME && c.Value == $"{dto.FirstName} {dto.LastName}");
            userClaims.ShouldContain(c => c.Type == JwtClaimConstants.GIVEN_NAME && c.Value == dto.FirstName);
            userClaims.ShouldContain(c => c.Type == JwtClaimConstants.FAMILY_NAME && c.Value == dto.LastName);
            userClaims.ShouldContain(c => c.Type == JwtClaimConstants.EMAIL && c.Value == dto.Email);
            var userAdded = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == dto.Email);
            userAdded.ShouldNotBeNull();
            userAdded.FirstName.ShouldBe(dto.FirstName);
            userAdded.LastName.ShouldBe(dto.LastName);
            userAdded.Email.ShouldBe(dto.Email);
            userAdded.Id.ShouldNotBe(default);
            userAdded.UserId.ShouldNotBe(Guid.Empty);
        }

        [Fact]
        public async Task Register_CreateNewAccountInvalidEmail_ShouldReturnBadRequestWithInfoInvalidEmail()
        {
            // Arrange
            var dto = new RegisterDTO("FirstNameTest", "LastNameTest", "invalidemailtest", "P@AsW0Rd!123!321");

            // Act
            var response = await Client.PostAsJsonAsync(URL, dto);

            // Assert
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ErrorMessage>();
            result.ShouldNotBeNull();
            result.Parameters.ShouldNotBeNull().ShouldNotBeEmpty();
            var expectedError = UserErrorMessages.InvalidEmail(dto.Email);
            result.Code.ShouldBe(expectedError.Code);
            result.Message.ShouldBe(expectedError.Message);
            result.Parameters.Count.ShouldBe(expectedError.Parameters!.Count);
            result.Parameters.First().Value.ToString().ShouldBe(dto.Email);
        }

        [Fact]
        public async Task Register_CreateNewAccountExistingEmail_ShouldReturnBadRequestWithGenericError()
        {
            // Arrange
            var dto = new RegisterDTO("FirstNameTest", "LastNameTest", "alicja@krainaczarow.com", "P@AsW0Rd!123!321");

            // Act
            var response = await Client.PostAsJsonAsync(URL, dto);

            // Assert
            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<ErrorMessage>();
            result.ShouldNotBeNull();
            result.ShouldBeEquivalentTo(UserErrorMessages.UserCannotBeCreated());
        }
    }
}
