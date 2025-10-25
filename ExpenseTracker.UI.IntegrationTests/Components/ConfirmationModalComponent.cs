using AngleSharp.Dom;
using Bunit;
using Microsoft.AspNetCore.Components.Web;

namespace ExpenseTracker.UI.IntegrationTests.Components
{
    public class ConfirmationModalComponent
    {
        private readonly IRenderedComponent<App> _component;

        public ConfirmationModalComponent(IRenderedComponent<App> component)
        {
            _component = component;
        }

        private IElement? TitleElement => _component.Find("div.mud-dialog-title");
        private IElement? MessageElement => _component.Find("div.mud-dialog-content");

        private IElement? YesButton => _component.FindAll("button").FirstOrDefault(b => b.ClassList.Contains("mud-message-box__yes-button"));
        private IElement? NoButton => _component.FindAll("button").FirstOrDefault(b => b.ClassList.Contains("mud-message-box__no-button"));

        public string? Title => TitleElement?.TextContent;
        public string? Message => MessageElement?.TextContent;

        public async Task ClickYes() => await YesButton!.ClickAsync(new MouseEventArgs());
        public void ClickNo() => NoButton?.Click();
    }
}
