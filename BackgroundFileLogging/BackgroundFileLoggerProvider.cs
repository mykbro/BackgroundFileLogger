using Microsoft.Extensions.Logging;


namespace BackgroundFileLogging
{
    public class BackgroundFileLoggerProvider : ILoggerProvider
    {
        //private readonly string _folderPath;
        private readonly BackgroundFileLogger _fileLoggerInstance;

        public BackgroundFileLoggerProvider(string folderPath)
        {
            _fileLoggerInstance = new BackgroundFileLogger(folderPath);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new BackgroundFileLoggerWrapper(_fileLoggerInstance, categoryName);
        }

        public void Dispose()
        {
            _fileLoggerInstance.Dispose();
        }
    }
}
