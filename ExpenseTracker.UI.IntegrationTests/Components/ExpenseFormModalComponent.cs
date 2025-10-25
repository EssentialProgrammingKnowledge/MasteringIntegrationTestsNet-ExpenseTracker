using AngleSharp.Dom;
using Bunit;
using ExpenseTracker.UI.Models;
using Microsoft.AspNetCore.Components.Web;
using System.Globalization;

namespace ExpenseTracker.UI.IntegrationTests.Components
{
    public class ExpenseFormModalComponent
    {
        private readonly IRenderedComponent<App> _component;

        public ExpenseFormModalComponent(IRenderedComponent<App> fragment)
        {
            _component = fragment;
        }

        private IElement? TitleText => _component.FindFirstOrDefault("[data-name='expense-form-title']");
        private IElement DescriptionInput => _component.Find("[data-name='expense-form-description']");
        private IElement AmountInput => _component.Find("[data-name='expense-form-amount']");
        private IElement CurrencySelect => _component.Find("[data-name='expense-form-currency']");
        private IElement? ExistingRateElement => _component.FindFirstOrDefault("[data-name='expense-form-existing-rate']");
        private IElement? RateElement => _component.FindFirstOrDefault("[data-name='expense-form-rate']");
        private IElement CategorySelect => _component.Find("[data-name='expense-form-category']");
        private IElement SubmitButton => _component.Find("[data-name='expense-form-submit']");
        private IElement CancelButton => _component.Find("[data-name='expense-form-cancel']");

        public bool IsVisible => TitleText is not null;
        public string? Title => TitleText?.TextContent;
        public string? Description => DescriptionInput?.GetAttribute("value");
        public string? Amount => AmountInput?.GetAttribute("value");
        public string? Currency => CurrencySelect?.GetAttribute("value");
        public bool HasRate => RateElement is not null;
        public bool HasExistingRate => ExistingRateElement is not null;
        public string? Rate => ExistingRateElement?.QuerySelector("[data-name='expense-form-rate-value']")?.TextContent ?? RateElement?.QuerySelector("[data-name='expense-form-rate-value']")?.TextContent;
        public string? TotalAmount => GetTotalAmount();
        public string? CategoryId => CategorySelect?.GetAttribute("value");
        public bool IsCurrencyDisabled => CurrencySelect?.HasAttribute("disabled") == true;
        public IEnumerable<string> FormErrors
            => _component.FindAll(".mud-input-helper-text.mud-input-error").Select(e => e.TextContent);

        public ExpenseFormModalComponent EnterDescription(string description)
        {
            DescriptionInput.Input(description);
            return this;
        }

        public ExpenseFormModalComponent EnterAmount(decimal amount)
        {
            AmountInput.Input(amount.ToString(CultureInfo.InvariantCulture));
            return this;
        }

        public ExpenseFormModalComponent SelectCurrency(int index)
        {
            ClickSelectControl(CurrencySelect);
            var options = GetSelectOptions();
            var option = options[index + 1];
            option?.Click();
            _component.Render();
            return this;
        }

        public ExpenseFormModalComponent SelectCurrencyByName(string currency)
        {
            ClickSelectControl(CurrencySelect);
            var options = GetSelectOptions();
            var option = options.FirstOrDefault(o => o.TextContent.Trim().Contains(currency))
                ?? throw new InvalidOperationException($"Currency with name {currency} not exists");
            option?.Click();
            _component.Render();
            return this;
        }

        public ExpenseFormModalComponent SelectCategory(int index)
        {
            ClickSelectControl(CategorySelect);
            var options = GetSelectOptions();
            var option = options[index];
            option?.Click();
            _component.Render();
            return this;
        }

        public ExpenseFormModalComponent SelectCategoryByCategoryName(string categoryName)
        {
            ClickSelectControl(CategorySelect);
            var options = GetSelectOptions();
            var option = options.FirstOrDefault(o => o.TextContent.Trim().Contains(categoryName))
                ?? throw new InvalidOperationException($"Category with name {categoryName} not exists");
            option?.Click();
            _component.Render();
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

        public ExpenseFormDTO GetFormValues()
        {
            _ = Enum.TryParse<Currency>(Currency, out var currency);
            _ = decimal.TryParse(Amount, out var amount);
            _ = int.TryParse(CategoryId, out var categoryId);
            var rate = (decimal?)null;
            var totalAmount = (decimal?)null;
            if (HasRate || HasExistingRate)
            {
                rate = Rate!.FromCurrencyStringToDecimal();
                totalAmount = TotalAmount!.FromCurrencyStringToDecimal();
            }

            return new()
            {
                Description = Description ?? string.Empty,
                Amount = amount,
                Rate = rate,
                TotalAmount = totalAmount,
                Currency = currency,
                CategoryId = categoryId
            };
        }

        private void ClickSelectControl(IElement element)
        {
            var selectWrapper = element.Closest(".mud-select")
                ?? throw new InvalidOperationException("Select changed structure, please take a look at MudBlazor implementation");
            selectWrapper.TriggerEvent("onmousedown", new MouseEventArgs());
            _component.Render();
        }

        private List<IElement> GetSelectOptions()
        {
            return [.. _component.FindAll("div.mud-popover-provider .mud-list-item")];
        }

        private string? GetTotalAmount()
        {
            var wrapper = _component.Find("[data-name='expense-form-amount']")
                           .Closest(".mud-input-control");
            var adornment = wrapper?.GetElementsByClassName("mud-input-adornment-end").FirstOrDefault();
            return adornment?.TextContent?.Trim();
        }
    }

    public record ExpenseFormDTO
    {
        public string Description { get; set; } = string.Empty;
        public decimal Amount  { get; set; } = decimal.Zero;
        public decimal? Rate { get; set; }
        public decimal? TotalAmount { get; set; }
        public Currency Currency  { get; set; }
        public int CategoryId { get; set; }
    }
}
