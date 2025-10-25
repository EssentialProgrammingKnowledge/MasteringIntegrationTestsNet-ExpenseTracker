using Bunit;
using ExpenseTracker.IntegrationTests.Setup;
using ExpenseTracker.UI.IntegrationTests.PageObjects;
using ExpenseTracker.UI.Models;
using ExpenseTracker.UI.Pages;
using Shouldly;

namespace ExpenseTracker.UI.IntegrationTests.Pages
{
    public class ProfilePageTests
    {
        private readonly TestFixture _testFixture;
        private readonly UserProfile _user;
        private ProfilePageObject profilePage = null!;

        public ProfilePageTests()
        {
            _testFixture = new TestFixture();
            _user = new UserProfile(1, Guid.NewGuid(), "email@email.com", "New", "User", "New User");
            _testFixture.TestContext.AuthContext.SetAuthorized(_user.FullName);
            _testFixture.TestContext.AuthContext.SetClaims(_user.CreateClaims());
            NavigateToProfile();
        }

        [Fact]
        public void ProfilePage_NoUserData_ShouldDisplayEmptyMessage()
        {
            // Arrange
            _testFixture.TestContext.AuthContext.SetClaims(null!);
            NavigateToProfile();

            // Act
            var hasEmpty = profilePage.HasEmptyUserData;

            // Assert
            hasEmpty.ShouldBeTrue();
        }

        [Fact]
        public void ProfilePage_WithUserData_ShouldDisplayUserInfo()
        {
            // Arrange Act Assert
            profilePage.FirstName.ShouldBe(_user.FirstName);
            profilePage.LastName.ShouldBe(_user.LastName);
            profilePage.Email.ShouldBe(_user.Email);
        }

        private void NavigateToProfile()
        {
            _testFixture.NavigateTo("/profile", true);
            var profile = _testFixture.AppComponent.FindComponent<Profile>();
            profilePage = new ProfilePageObject(profile);
        }
    }
}
