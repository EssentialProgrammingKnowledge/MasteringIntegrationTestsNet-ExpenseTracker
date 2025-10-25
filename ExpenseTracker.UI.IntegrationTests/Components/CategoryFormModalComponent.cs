using AngleSharp.Dom;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;

namespace ExpenseTracker.UI.IntegrationTests.Components
{
    public class CategoryFormModalComponent
    {
        private readonly IRenderedComponent<App> _component;

        public CategoryFormModalComponent(IRenderedComponent<App> fragment)
        {
            _component = fragment;
        }

        private IElement? TitleText => _component.FindFirstOrDefault("[data-name='category-form-title']");
        private IElement NameInput => _component.Find("[data-name='category-form-name']");
        private IElement BudgetInput => _component.Find("[data-name='category-form-budget']");
        private IElement SubmitButton => _component.Find("[data-name='category-form-submit']");
        private IElement CancelButton => _component.Find("[data-name='category-form-cancel']");

        public bool IsVisible => TitleText is not null;
        public string? Title => TitleText?.TextContent;
        public string? Name => NameInput?.GetAttribute("value");
        public string? Budget => BudgetInput?.GetAttribute("value");
        public IEnumerable<string> FormErrors
            => _component.FindAll(".mud-input-helper-text.mud-input-error").Select(e => e.TextContent);

        public CategoryFormModalComponent EnterName(string name)
        {
            NameInput.Input(name);
            return this;
        }

        public CategoryFormModalComponent EnterBudget(decimal budget)
        {
            BudgetInput.Input(budget.ToString(CultureInfo.InvariantCulture));
            return this;
        }

        public async Task ClickSubmit()
        {
            await SubmitButton.ClickAsync(new MouseEventArgs());
        }

        public void ClickCancel()
        {
            CancelButton.Click();
        }

        public CategoryFormDTO GetFormValues()
        {
            _ = decimal.TryParse(Budget, out var budget);
            return new()
            {
                Name = Name ?? string.Empty,
                Budget = budget
            };
        }
    }

    public record CategoryFormDTO
    {
        public string Name { get; set; } = string.Empty;
        public decimal Budget { get; set; }
    }
}
