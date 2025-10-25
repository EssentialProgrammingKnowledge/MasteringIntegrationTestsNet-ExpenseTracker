using AngleSharp.Dom;
using Bunit;

namespace ExpenseTracker.UI.IntegrationTests.Components
{
    public class NavbarComponent
    {
        private readonly IRenderedComponent<App> _app;

        public NavbarComponent(IRenderedComponent<App> app)
        {
            _app = app;
        }

        private IElement? ProfileButton => _app.FindFirstOrDefault("[data-name='profile-button']");
        private IElement? LogoutButton => _app.FindFirstOrDefault("[data-name='logout-button']");
        private IElement? ProfileTooltip => _app.FindFirstOrDefault("[data-name='profile-tooltip']");
        private IElement? FullNameTooltip => _app.FindFirstOrDefault("[data-name='profile-fullname-tooltip']");
        private IElement? FullNameText => _app.FindFirstOrDefault("[data-name='profile-fullname']");

        public void ClickProfile()
        {
            ProfileButton?.Click();
        }

        public void ClickLogout()
        {
            LogoutButton?.Click();
            _app.Render();
        }

        public bool IsProfileVisible => ProfileButton is not null;
        public bool IsLogoutVisible => LogoutButton is not null;

        public string? ProfileTooltipText => ProfileTooltip?.TextContent.Trim();

        public string? DisplayedUserName => FullNameText?.TextContent.Trim();

        public string? DisplayedUserNameTooltip => FullNameTooltip?.GetAttribute("text");
    }
}
