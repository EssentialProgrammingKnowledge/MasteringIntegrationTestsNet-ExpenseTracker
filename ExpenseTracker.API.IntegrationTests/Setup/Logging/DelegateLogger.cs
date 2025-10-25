using Microsoft.Extensions.Logging;

namespace ExpenseTracker.API.IntegrationTests.Setup.Logging
{
    internal class DelegateLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly Action<string, LogLevel, string> _logAction;

        public DelegateLogger(string categoryName, Action<string, LogLevel, string> logAction)
        {
            _categoryName = categoryName;
            _logAction = logAction;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
            => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel)
            => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var message = formatter(state, exception);
            if (exception != null)
            {
                message += Environment.NewLine + exception.ToString();
            }

            _logAction?.Invoke(_categoryName, logLevel, message);
        }

        private class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new NullScope();

            public void Dispose()
            { }
        }
    }
}
