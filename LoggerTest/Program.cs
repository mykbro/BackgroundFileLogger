using BackgroundFileLogging;
using Microsoft.Extensions.Logging;

namespace LoggerTest
{
    internal class Program
    {
        static void Main(string[] args)
        {            
            using (var logProvider = new BackgroundFileLoggerProvider("D:\\Logs"))  //<-- insert a valid folder path
            {
                var logger = logProvider.CreateLogger(categoryName: "MyCat");

                using (logger.BeginScope("Scope 1"))
                {
                    using (logger.BeginScope("Scope 2"))
                    {
                        logger.LogInformation("Just logging something");
                    }                       
                }
            }

            Console.WriteLine("Press a key to end the program");
            Console.ReadKey();
        }
    }
}