using AngleSharp.Dom;
using Bunit;
using ExpenseTracker.UI.IntegrationTests.Components;
using ExpenseTracker.UI.Models;
using ExpenseTracker.UI.Pages;
using Shouldly;

namespace ExpenseTracker.UI.IntegrationTests.PageObjects
{
    public class HomePageObject
    {
        private readonly IRenderedComponent<Home> _component;

        public HomePageObject(IRenderedComponent<Home> component)
        {
            _component = component;
        }

        private IElement AddButton => _component.Find("[data-name='expenses-add-button']");
        private IElement SearchInput => _component.Find("[data-name='expenses-search-input']");
        private IElement ExpensesTable => _component.Find("[data-name='expenses-table']");

        private IReadOnlyList<IElement> Rows => _component.FindAll("[data-name='expenses-table'] tbody tr");
        private IElement? LoadingIcon => _component.FindAll("[data-name='expenses-loading-icon']").FirstOrDefault();

        public bool IsLoading => LoadingIcon != null;
        public bool HasExpensesTable => ExpensesTable != null && !IsLoading;
        public int ExpensesCount => Rows.Count;
        public IEnumerable<ExpensePageRowDTO> Expenses => GetExpenses();

        public ExpensePageRowDTO? GetExpenseByDescription(string description) =>
            Expenses.FirstOrDefault(r => r.Description == description);

        public ExpenseFormModalComponent ClickAddExpense(IRenderedComponent<App> component)
        {
            AddButton?.Click();
            component.Render();
            var formModalComponent = new ExpenseFormModalComponent(component);
            component.WaitForAssertion(() =>
            {
                formModalComponent.IsVisible.ShouldBeTrue();
            }, TimeSpan.FromSeconds(5));
            return formModalComponent;
        }

        public void SearchExpense(string query)
        {
            SearchInput.Change(query);
        }

        private static ExpensePageRowDTO ParseRow(IElement row)
        {
            var id = int.Parse(row.QuerySelector("[data-name='expenses-column-id']")?.TextContent ?? "0");
            var description = row.QuerySelector("[data-name='expenses-column-description']")?.TextContent ?? string.Empty;
            _ = Enum.TryParse<Currency>(row.QuerySelector("[data-name='expenses-column-currency']")?.TextContent ?? string.Empty, out var currency);
            var totalAmount = (row.QuerySelector("[data-name='expenses-column-total-amount']")?.TextContent ?? string.Empty).FromCurrencyStringToDecimal();
            var amount = (row.QuerySelector("[data-name='expenses-column-amount']")?.TextContent ?? string.Empty).FromCurrencyStringToDecimal(currency);
            var rate = (row.QuerySelector("[data-name='expenses-column-rate']")?.TextContent ?? string.Empty).FromCurrencyStringToDecimal(currency);
            _ = int.TryParse(row.QuerySelector("[data-name='expenses-column-category-id']")?.TextContent ?? string.Empty, out var categoryId);
            var editButton = row.QuerySelector("[data-name='expenses-column-edit']");
            var deleteButton = row.QuerySelector("[data-name='expenses-column-delete']");

            return new ExpensePageRowDTO(
                id,
                totalAmount,
                amount,
                currency,
                rate,
                description,
                categoryId,
                (IRenderedComponent<App> component) => {
                    editButton!.Click();
                    component.Render();
                    var formModalComponent = new ExpenseFormModalComponent(component);
                    component.WaitForAssertion(() =>
                    {
                        formModalComponent.IsVisible.ShouldBeTrue();
                    }, TimeSpan.FromSeconds(5));
                    return formModalComponent;
                },
                (IRenderedComponent<App> component) => {
                    deleteButton!.Click();
                    component.Render();
                    var confirmationModalComponent = new ConfirmationModalComponent(component);
                    component.WaitForAssertion(() =>
                    {
                        confirmationModalComponent.Message.ShouldNotBeNullOrWhiteSpace();
                    }, TimeSpan.FromSeconds(5));
                    return confirmationModalComponent;
                }
            );
        }

        private IEnumerable<ExpensePageRowDTO> GetExpenses()
        {
            foreach (var row in Rows)
            {
                yield return ParseRow(row);
            }
        }
    }

    public record ExpensePageRowDTO(
        int Id,
        decimal TotalAmount,
        decimal Amount,
        Currency Currency,
        decimal Rate,
        string Description,
        int CategoryId,
        Func<IRenderedComponent<App>, ExpenseFormModalComponent> Edit,
        Func<IRenderedComponent<App>, ConfirmationModalComponent> Delete
    );
}
