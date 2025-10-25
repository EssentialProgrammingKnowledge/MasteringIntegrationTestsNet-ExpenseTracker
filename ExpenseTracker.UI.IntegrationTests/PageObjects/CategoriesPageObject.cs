using AngleSharp.Dom;
using Bunit;
using ExpenseTracker.UI.IntegrationTests.Components;
using ExpenseTracker.UI.Pages;
using Shouldly;

namespace ExpenseTracker.UI.IntegrationTests.PageObjects
{
    public class CategoriesPageObject
    {
        private readonly IRenderedComponent<Categories> _component;

        public CategoriesPageObject(IRenderedComponent<Categories> component)
        {
            _component = component;
        }

        private IElement AddButton => _component.Find("[data-name='categories-add-button']");
        private IElement SearchInput => _component.Find("[data-name='categories-search-input']");
        private IElement CategoriesTable => _component.Find("[data-name='categories-table']");

        private IReadOnlyList<IElement> Rows => _component.FindAll("[data-name='categories-table'] tbody tr");
        private IElement? LoadingIcon => _component.FindFirstOrDefault("[data-name='categories-loading-icon']");

        public bool IsLoading => LoadingIcon != null;
        public bool HasCategoriesTable => CategoriesTable != null && !IsLoading;
        public int CategoriesCount => Rows.Count;
        public IEnumerable<CategoryPageRowDTO> Categories => GetCategories();

        public CategoryPageRowDTO? GetCategoryByName(string name) =>
            Categories.FirstOrDefault(r => r.Name == name);

        public CategoryFormModalComponent ClickAddCategory(IRenderedComponent<App> component)
        {
            AddButton?.Click();
            component.Render();
            var formModalComponent = new CategoryFormModalComponent(component);
            component.WaitForAssertion(() =>
            {
                formModalComponent.IsVisible.ShouldBeTrue();
            });
            return formModalComponent;
        }

        public void SearchCategory(string query)
        {
            SearchInput.Change(query);
        }

        private static CategoryPageRowDTO ParseRow(IElement row)
        {
            var id = int.Parse(row.QuerySelector("[data-name='categories-column-id']")?.TextContent ?? "0");
            var name = row.QuerySelector("[data-name='categories-column-name']")?.TextContent ?? string.Empty;
            var budget = (row.QuerySelector("[data-name='categories-column-budget']")?.TextContent ?? string.Empty).FromCurrencyStringToDecimal();

            var editButton = row.QuerySelector("[data-name='categories-column-edit']");
            var deleteButton = row.QuerySelector("[data-name='categories-column-delete']");

            return new CategoryPageRowDTO(
                id,
                name,
                budget,
                (IRenderedComponent<App> component) => {
                    editButton!.Click();
                    component.Render();
                    var formModalComponent = new CategoryFormModalComponent(component);
                    component.WaitForAssertion(() =>
                    {
                        formModalComponent.IsVisible.ShouldBeTrue();
                    });
                    return formModalComponent;
                },
                (IRenderedComponent<App> component) => {
                    deleteButton!.Click();
                    component.Render();
                    var confirmationModalComponent = new ConfirmationModalComponent(component);
                    component.WaitForAssertion(() =>
                    {
                        confirmationModalComponent.Message.ShouldNotBeNullOrWhiteSpace();
                    });
                    return confirmationModalComponent;
                }
            );
        }

        private IEnumerable<CategoryPageRowDTO> GetCategories()
        {
            foreach (var row in Rows)
            {
                yield return ParseRow(row);
            }
        }
    }

    public record CategoryPageRowDTO(
        int Id,
        string Name,
        decimal Budget,
        Func<IRenderedComponent<App>, CategoryFormModalComponent> Edit,
        Func<IRenderedComponent<App>, ConfirmationModalComponent> Delete
    );
}
