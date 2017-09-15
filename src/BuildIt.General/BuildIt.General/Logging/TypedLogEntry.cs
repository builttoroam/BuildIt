using System;
using System.Collections.Generic;

namespace BuildIt.Logging
{
    /// <summary>
    /// Typed log entry which includes an entity to log out
    /// </summary>
    /// <typeparam name="TEntity">The type of entity</typeparam>
    internal class TypedLogEntry<TEntity> : LogEntry, ITypedLogEntry<TEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypedLogEntry{TEntity}"/> class.
        /// </summary>
        /// <param name="level">The log level</param>
        /// <param name="assemblyName">The assembly name of the code that logged the message</param>
        /// <param name="method">The method that invoked the log</param>
        /// <param name="entity">The entity to be logged</param>
        /// <param name="message">The message</param>
        /// <param name="exception">The exception</param>
        /// <param name="metaData">The meta data</param>
        /// <param name="categories">The categories</param>
        public TypedLogEntry(LogLevel level, string assemblyName, string method, TEntity entity, string message, Exception exception, IDictionary<string, string> metaData, params string[] categories)
            : base(level, method, assemblyName, message, exception, metaData, categories)
        {
            Entity = entity;
        }

        /// <summary>
        /// Gets  the entity to log
        /// </summary>
        public TEntity Entity { get; }
    }
}