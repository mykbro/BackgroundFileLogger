using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Collections.Concurrent;
using System.Text;


namespace BackgroundFileLogging
{
    public class BackgroundFileLogger : IDisposable
    {
        private const string Extension = ".txt";

        private static readonly FileStreamOptions DefaultFileStreamOptions = new FileStreamOptions()
        {
            Access = FileAccess.Write,
            Share = FileShare.Read,
            Mode = FileMode.Append
        };       

        private readonly string _folderPath;
        private readonly BlockingCollection<string> _msgBlockingQueue;      

        private StreamWriter? _streamWriter;
        private string? _currentFileName;


        public BackgroundFileLogger(string folderPath)
        {
            Directory.CreateDirectory(folderPath);  // create if not already exists and throws if path is invalid
            _folderPath = folderPath;

            _msgBlockingQueue = new BlockingCollection<string>();

            //we spawn the thread responsible for the writing itself
            Thread backgroundThread = new Thread(KeepWaitingForMessagesAndWriteThemDown);
            backgroundThread.IsBackground = true;
            backgroundThread.Start();                

            _streamWriter = null;
            _currentFileName = null;
        }        

        public void Dispose()
        {
            _msgBlockingQueue.Dispose();
            _streamWriter?.Dispose();
        } 

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter, string? category, LoggingScope? logScope)
        {           
            StringBuilder sb = new StringBuilder();
            sb.Append($"[{logLevel}] | {DateTimeOffset.Now} | ");

            if (category != null)
            {
                sb.Append("Category: ");
                sb.Append(category);
                sb.Append(" | ");
            }

            if (logScope != null)
            {
                sb.Append("Scope: ");
                sb.Append(logScope.ToString());
                sb.Append(" | ");
            }

            sb.Append(state?.ToString());

            _msgBlockingQueue.Add(sb.ToString());            
        }

        private void KeepWaitingForMessagesAndWriteThemDown()
        {
            bool exit = false;

            while (!exit)
            {
                try
                {
                    //we wait until we have a msg
                    string msgToWrite = _msgBlockingQueue.Take();

                    //we setup the correct file
                    SetupLogFile();

                    //we write and immediately flush the msg
                    if (_streamWriter != null)
                    {
                        _streamWriter.WriteLine(msgToWrite);
                        _streamWriter.Flush();
                    }
                    
                }
                catch
                {
                    //we'll get here on disposing the blocking queue... this will end the thread
                    exit = true;
                }
            }
        }

        private void SetupLogFile()
        {

            string todayAsString = DateTime.Now.ToString("yyyy-MM-dd");

            // we check if we're using the wrong file (the day is over) or we're not yet using a file (just started)
            if (_currentFileName != todayAsString)
            {
                // if we had a file open we close it               
                _streamWriter?.Dispose();

                _streamWriter = new StreamWriter(Path.Combine(_folderPath, todayAsString + Extension), DefaultFileStreamOptions);
                _currentFileName = todayAsString;
            }
        }
    }
}
