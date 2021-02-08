using System;
using System.Diagnostics;
using RequestProcessor.App.Logging;

namespace RequestProcessor.App
{
    class Logger : ILogger
    {
        public void Log(string message)
        {
            Debug.WriteLine(message);
        }

        public void Log(Exception exception, string message)
        {
            Debug.WriteLine($"Exception:{exception} -> Message: {message}");
        }
    }
}
