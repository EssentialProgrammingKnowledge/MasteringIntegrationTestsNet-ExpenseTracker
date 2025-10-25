using AngleSharp.Dom;
using Bunit;

namespace ExpenseTracker.UI.IntegrationTests.Components
{
    public class SnackbarComponent
    {
        private readonly IRenderedFragment _rendered;

        public SnackbarComponent(IRenderedFragment rendered)
        {
            _rendered = rendered;
        }

        private IElement Container => _rendered.Find("#mud-snackbar-container");

        private IReadOnlyList<IElement> Snackbars => Container.QuerySelectorAll(".mud-snackbar").Reverse().ToList();

        public int Count => Snackbars.Count;

        public string GetMessage(int index = 0)
        {
            var snackbar = Snackbars[index];
            return snackbar.QuerySelector(".mud-snackbar-content-message")?.TextContent?.Trim() ?? string.Empty;
        }

        public string GetSeverity(int index = 0)
        {
            var snackbar = Snackbars[index];

            if (snackbar.ClassList.Contains("mud-alert-filled-error"))
            {
                return "error";
            }

            if (snackbar.ClassList.Contains("mud-alert-filled-success"))
            {
                return "success";
            }

            if (snackbar.ClassList.Contains("mud-alert-filled-info"))
            {
                return "info";
            }

            if (snackbar.ClassList.Contains("mud-alert-filled-warning"))
            {
                return "warning";
            }

            return "unknown";
        }

        public void Close(int index = 0)
        {
            var snackbar = Snackbars[index];
            snackbar.QuerySelector(".mud-snackbar-close-button")?.Click();
        }
    }
}
