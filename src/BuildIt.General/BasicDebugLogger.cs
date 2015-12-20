using System;

namespace BuildIt
{
    public class BasicDebugLogger : ILogService
    {
        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Exception(string message, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Exception:" + ex.Message + " - " + message);
        }
    }
}