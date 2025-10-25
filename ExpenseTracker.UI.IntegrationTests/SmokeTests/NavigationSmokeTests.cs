using ExpenseTracker.IntegrationTests.Setup;
using ExpenseTracker.UI.IntegrationTests.Components;
using Shouldly;

namespace ExpenseTracker.UI.IntegrationTests.SmokeTests
{
    public class NavigationSmokeTests
    {
        private readonly TestFixture _testFixture = new();

        [Theory]
        [InlineData("/", "/login")]
        [InlineData("/categories", "/login")]
        [InlineData("/profile", "/login")]
        public void NavigationTest_UnauthenticatedAccess_ShouldRedirectToLogin(string path, string expectedPath)
        {
            // Act
            _testFixture.NavigateTo(path);

            // Assert
            _testFixture.CurrentUrl.ShouldContain(expectedPath);
        }

        [Fact]
        public void NavigationTest_UnauthenticatedAccess_ShouldHideNavMenu()
        {
            // Act
            _testFixture.NavigateTo("/categories");

            // Assert
            var navMenu = new NavMenuComponent(_testFixture.AppComponent, _testFixture.NavigateTo);
            navMenu.IsVisible.ShouldBeFalse();
        }

        [Fact]
        public void NavigationTest_UnauthenticatedAccess_ShouldHideMostOfActionOfNavbar()
        {
            // Act
            _testFixture.NavigateTo("/categories");

            // Assert
            var navbar = new NavbarComponent(_testFixture.AppComponent);
            navbar.DisplayedUserNameTooltip.ShouldBeNull();
            navbar.IsLogoutVisible.ShouldBeFalse();
            navbar.IsProfileVisible.ShouldBeFalse();
            navbar.ProfileTooltipText.ShouldBeNull();
        }
    }
}
