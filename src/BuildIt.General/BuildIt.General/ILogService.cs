using System;

namespace BuildIt
{
    /// <summary>
    /// Interface for the log service
    /// </summary>
    public interface ILogService
    {
        /// <summary>
        /// Writes out debugging information
        /// </summary>
        /// <param name="message">The message to write</param>
        void Debug(string message);

        /// <summary>
        /// Writes out exception information
        /// </summary>
        /// <param name="message">The message to write</param>
        /// <param name="ex">The exception to write</param>
        void Exception(string message, Exception ex);
    }
}