using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuildIt.Logging
{
    /// <summary>
    /// A entry to place in the log
    /// </summary>
    internal class LogEntry : ILogEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        /// <param name="level">The log level</param>
        /// <param name="assemblyName">The assembly name of the code that logged the message</param>
        /// <param name="method">The method that invoked the log</param>
        /// <param name="message">The message</param>
        /// <param name="exception">The exception</param>
        /// <param name="metaData">The meta data</param>
        /// <param name="categories">The categories</param>
        public LogEntry(LogLevel level, string assemblyName, string method, string message, Exception exception, IDictionary<string, string> metaData, params string[] categories)
        {
            Timestamp = DateTime.UtcNow;
            AssemblyName = assemblyName;
            Level = level;
            Method = method;
            Message = message;
            Exception = exception;
            Metadata = metaData;
            Categories = categories;
        }

        /// <summary>
        /// Gets the assembly name of the caller
        /// </summary>
        public string AssemblyName { get; }

        /// <summary>
        /// Gets timestamp (UTC) of the log event
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Gets the message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the categories
        /// </summary>
        public string[] Categories { get; }

        /// <summary>
        /// Gets the method of the caller
        /// </summary>
        public string Method { get; }

        /// <summary>
        /// Gets the exception
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets the log level
        /// </summary>
        public LogLevel Level { get; }

        /// <summary>
        /// Gets the metadata
        /// </summary>
        public IDictionary<string, string> Metadata { get; }

        /// <summary>
        /// Provides a string representation of the log message
        /// </summary>
        /// <returns>The log as text</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Timestamp.ToString("s") + ": [");
            if (!string.IsNullOrWhiteSpace(AssemblyName))
            {
                sb.Append(AssemblyName);
            }

            sb.Append(Method + "] " + Level);

            if (!string.IsNullOrWhiteSpace(Message))
            {
                sb.Append(" - " + Message);
            }

            if (Exception != null)
            {
                sb.AppendLine(Exception.Message);
            }

            if (Categories?.Any() ?? false)
            {
                sb.AppendLine("(" + string.Join(", ", Categories) + ")");
            }

            if (Metadata?.Any() ?? false)
            {
                sb.AppendLine("Data [" + string.Join(", ", Metadata.Select(x => x.Key + "=" + x.Value) + "]"));
            }

            if (Exception != null)
            {
                sb.AppendLine("--------- STACK TRACE ----------");
                sb.AppendLine(Exception.StackTrace);
                sb.AppendLine("--------------------------------");
            }

            return sb.ToString();
        }
    }
}