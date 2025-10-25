using Microsoft.Extensions.Logging;

namespace ExpenseTracker.API.IntegrationTests.Setup.Logging
{
    internal class DelegateLoggerProvider : ILoggerProvider
    {
        private readonly Action<string, LogLevel, string> _logAction;

        public DelegateLoggerProvider(Action<string, LogLevel, string> logAction)
        {
            _logAction = logAction;
        }

        public ILogger CreateLogger(string categoryName)
            => new DelegateLogger(categoryName, _logAction);

        public void Dispose() { }
    }
}
