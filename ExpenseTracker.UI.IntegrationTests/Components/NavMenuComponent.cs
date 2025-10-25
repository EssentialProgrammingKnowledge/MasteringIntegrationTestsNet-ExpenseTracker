using AngleSharp.Dom;
using Bunit;
using Shouldly;

namespace ExpenseTracker.UI.IntegrationTests.Components
{
    public class NavMenuComponent
    {
        private readonly IRenderedComponent<App> _app;
        private readonly Action<string, bool> _navigateAction;

        public NavMenuComponent(IRenderedComponent<App> app, Action<string, bool> navigateAction)
        {
            _app = app;
            _navigateAction = navigateAction;
        }

        private IElement ToggleButton => _app.Find("button.navbar-toggler");
        public bool IsVisible => _app.FindAll("nav.nav.flex-column").Any();

        public void ToggleMenu()
        {
            ToggleButton.Click();
        }

        public void GoToHome()
        {
            var link = _app.FindAll("a[href='']").FirstOrDefault(l => l.TextContent.Contains("Strona główna"));
            link.ShouldNotBeNull();
            var href = link.Attributes.FirstOrDefault(a => a.Name.ToLower() == "href");
            link.Click();
            _navigateAction(href?.Value ?? "/", false);
        }

        public void GoToCategories()
        {
            var link = _app.Find("a[href='categories']");
            var href = link.Attributes.FirstOrDefault(a => a.Name.ToLower() == "href");
            link.Click();
            _navigateAction(href?.Value ?? "/", false);
        }
    }
}
