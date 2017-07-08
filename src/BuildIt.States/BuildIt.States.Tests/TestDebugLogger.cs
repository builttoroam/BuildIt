using System;

namespace BuildIt.States.Tests
{
    public class TestDebugLogger : BasicDebugLogger
    {
        public override void Debug(string message) => System.Diagnostics.Debug.WriteLine(message);

        public override void Exception(string message, Exception ex) => System.Diagnostics.Debug.WriteLine(message + " " + ex.StackTrace);
    }
}