using Microsoft.Extensions.Logging;

namespace BackgroundFileLogging
{
    public class BackgroundFileLoggerWrapper : ILogger
    {
        private readonly BackgroundFileLogger _logger;
        private readonly string _categoryName;
        private readonly LoggingScope _scope;

        public BackgroundFileLoggerWrapper(BackgroundFileLogger logger, string categoryName)
        {
            _logger = logger;
            _categoryName = categoryName;
            _scope = new LoggingScope();            
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            _scope.AddScope(state.ToString() ?? throw new ArgumentNullException(nameof(state)));
            return _scope;
        }        

        public bool IsEnabled(LogLevel logLevel)
        {
            //we enable all levels
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            //we call the internal singleton logger's Log method passing our instance category and scope
            _logger.Log(logLevel, eventId, state, exception, formatter, _categoryName, _scope);
        }
    }
}
