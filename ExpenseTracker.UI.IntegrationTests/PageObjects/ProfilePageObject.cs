using AngleSharp.Dom;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace ExpenseTracker.UI.IntegrationTests.PageObjects
{
    public class ProfilePageObject
    {
        private readonly IRenderedComponent<IComponent> _component;

        public ProfilePageObject(IRenderedComponent<IComponent> component)
        {
            _component = component;
        }

        private IElement? FirstNameElement => _component.FindFirstOrDefault("[data-name='profile-first-name']");
        private IElement? LastNameElement => _component.FindFirstOrDefault("[data-name='profile-last-name']");
        private IElement? EmailElement => _component.FindFirstOrDefault("[data-name='profile-email']");
        private IElement? EmptyUserDataElement => _component.FindFirstOrDefault("[data-name='profile-empty-user-data']");

        public bool HasEmptyUserData => EmptyUserDataElement != null;

        public string? FirstName => FirstNameElement?.TextContent.Replace("Imię:", "").Trim();
        public string? LastName => LastNameElement?.TextContent.Replace("Nazwisko:", "").Trim();
        public string? Email => EmailElement?.TextContent.Replace("Email:", "").Trim();
    }
}
