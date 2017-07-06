using System;

namespace BuildIt
{
    /// <summary>
    /// Abstract implementation of the ILogService - useful when debugging the
    /// General library during development. Methods need to be overridden
    /// if using this class in a project
    /// </summary>
    public class BasicDebugLogger : ILogService
    {
        /// <summary>
        /// Writes information to the debugger
        /// </summary>
        /// <param name="message">The message to write</param>
        public virtual void Debug(string message)
        {
            // NB: This is a null operation in the release method - this stub is only
            // here to facilitate debugging the General library
            System.Diagnostics.Debug.WriteLine(message);
        }

        /// <summary>
        /// Writes exception information to debugger
        /// </summary>
        /// <param name="message">The message to write</param>
        /// <param name="ex">The exception to write out</param>
        public virtual void Exception(string message, Exception ex)
        {
            // NB: This is a null operation in the release method - this stub is only
            // here to facilitate debugging the General library
            System.Diagnostics.Debug.WriteLine("Exception:" + ex.Message + " - " + message);
        }
    }
}