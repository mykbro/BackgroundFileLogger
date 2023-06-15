# BackgroundFileLogger

## Disclaimer
This codebase is made for self-teaching and educational purposes only.
Many features like input validation, object disposed checks, some exception handling, etc... are mostly missing.
As such this codebase cannot be considered production ready.

## What's this ?
This solution consists of:
* A simple library that provides a .Net Core background file logger that asynchronously logs messages.
* A driving console app.

## How does it work ?
The problem with ILogger interface is that it doesn't support async methods, so for blocking loggers like a file based one, we cannot even use StreamWriter::WriteAsync() or SqlCommand::ExecuteNonQueryAsync().  
A solution is to spawn a background worker thread and use a producer/consumer queue. This way, the calling method will not have to wait for the log to finish.  

The BackgroundFileLoggerProvider uses a singleton BackgroundFileLogger that spawns a background Thread on construction.  
However, to use the "category string" and the "logging scopes", which are instance based, we cannot return a singleton so we must use a wrapper that will "capture" the category/scopes and delegate the log itself to the singleton.  
If we do this, it is the wrapper that must implement the ILogger interface; the singleton can be a normal class.

The file logger will save its files inside a specified folder. Every day will have its separate log file.  
This is achieved by checking the current Datetime and the current open file every time a message should be logged.

## How should I use this ?
For .Net Core Web apps you must register the logger provider:

	builder.Services.AddLogging(builder =>
    {
        string myLogFolderPath = ... ;
        builder.AddProvider(new BackgroundFileLoggerProvider(myLogFolderPath));
    });

if you want to use the driving app please insert a valid folder path in Program.Main:

    static void Main(string[] args)
    {            
        using (var logProvider = new BackgroundFileLoggerProvider("D:\\Logs"))  //<-- insert a valid folder path
        { 
            ...
        }

        ...
    }