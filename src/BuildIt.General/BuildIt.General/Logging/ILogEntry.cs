using System;
using System.Collections.Generic;

namespace BuildIt.Logging
{
    /// <summary>
    /// A log entry.
    /// </summary>
    public interface ILogEntry
    {
        /// <summary>
        /// Gets the assembly name of the caller.
        /// </summary>
        string AssemblyName { get; }

        /// <summary>
        /// Gets the timestamp of the log entry.
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// Gets the message to be logged.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Gets the categories to be logged.
        /// </summary>
        string[] Categories { get; }

        /// <summary>
        /// Gets the method of the caller.
        /// </summary>
        string Method { get; }

        /// <summary>
        /// Gets the exception to be logged.
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// Gets the log level to be logged.
        /// </summary>
        LogLevel Level { get; }

        /// <summary>
        /// Gets the metadata to be logged.
        /// </summary>
        IDictionary<string, string> Metadata { get; }
    }
}