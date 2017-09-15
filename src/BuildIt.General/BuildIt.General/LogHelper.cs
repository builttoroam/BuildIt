using BuildIt.Logging;
using BuildIt.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace BuildIt
{
    /// <summary>
    /// Helper class that simplifies writing log information
    /// </summary>
    public static class LogHelper
    {
        private static ILoggerService logService;
        private static bool hasLookedForLogService;
        private static int wakeUpLock;

        /// <summary>
        /// Gets or sets the LogService instance
        /// </summary>
        public static ILoggerService LogService
        {
            get
            {
                try
                {
                    if (hasLookedForLogService)
                    {
                        return logService;
                    }

                    hasLookedForLogService = true;

                    if (!ServiceLocator.IsLocationProviderSet)
                    {
                        return null;
                    }

                    return logService ?? (LogService = ServiceLocator.Current.GetInstance<ILoggerService>());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error retrieving ILogService implementation: " + ex.Message);
                    return null;
                }
            }

            set
            {
                logService = value;
                hasLookedForLogService = true;
            }
        }

        private static Queue<ILogEntry> LogQueue { get; } = new Queue<ILogEntry>();

        private static AutoResetEvent LogWaiter { get; } = new AutoResetEvent(false);

        /// <summary>
        /// Logs information about an entity
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to write</typeparam>
        /// <param name="entity">The entity to write (serialized)</param>
        /// <param name="message">The message to be logged (optional)</param>
        /// <param name="categories">The category to be logged  (optional)</param>
        /// <param name="metadata">The metadata to be logged  (optional)</param>
        /// <param name="level">The log level to log (optional but defaults to Information)</param>
        /// <param name="assembly">The assembly name to be logged  (optional) </param>
        /// <param name="caller">The name of the calling method  (optional but defaults to the caller method name)</param>
        public static void LogEntity<TEntity>(this TEntity entity, string message = null, string[] categories = null, IDictionary<string, string> metadata = null, LogLevel level = LogLevel.Information, Assembly assembly = null, [CallerMemberName] string caller = null)
        {
            try
            {
                var entry = new TypedLogEntry<TEntity>(level, assembly?.GetName().Name, caller, entity, message, null, metadata, categories);
                InternalWriteLog(entry);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="message">The message to be logged (optional)</param>
        /// <param name="categories">The category to be logged  (optional)</param>
        /// <param name="metadata">The metadata to be logged  (optional)</param>
        /// <param name="level">The log level to log (optional but defaults to Information)</param>
        /// <param name="assembly">The assembly name to be logged  (optional) </param>
        /// <param name="caller">The name of the calling method  (optional but defaults to the caller method name)</param>
        public static void Log(this string message, string[] categories = null, IDictionary<string, string> metadata = null, LogLevel level = LogLevel.Information, Assembly assembly = null, [CallerMemberName] string caller = null)
        {
            try
            {
                var entry = new LogEntry(level, assembly?.GetName().Name, caller, message, null, metadata, categories);
                InternalWriteLog(entry);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Logs an exception
        /// </summary>
        /// <param name="exception">The exception to log</param>
        /// <param name="message">The message (optional) to log</param>
        /// <param name="categories">The category to be logged  (optional)</param>
        /// <param name="metadata">The metadata to be logged  (optional)</param>
        /// <param name="level">The log level to log (optional but defaults to Information)</param>
        /// <param name="assembly">The assembly name to be logged  (optional) </param>
        /// <param name="caller">The calling method</param>
        public static void LogException(this Exception exception, string message = null, string[] categories = null, IDictionary<string, string> metadata = null, LogLevel level = LogLevel.Error, Assembly assembly = null, [CallerMemberName] string caller = null)
        {
            try
            {
                var entry = new LogEntry(level, assembly?.GetName().Name, caller, message, exception, metadata, categories);
                InternalWriteLog(entry);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static void InternalWriteLog(ILogEntry log)
        {
            try
            {
                if (LogService == null || log == null)
                {
                    return;
                }

                LogQueue.Enqueue(log);
                if (Interlocked.CompareExchange(ref wakeUpLock, 1, 0) == 0)
                {
                    Task.Run(() => WakeUp());
                }
                else
                {
                    LogWaiter.Set();
                }
            }
            catch (Exception ext)
            {
                Debug.WriteLine(ext.Message);
            }
        }

        private static async void WakeUp()
        {
            try
            {
                while (LogQueue.Count > 0)
                {
                    var entry = LogQueue.Dequeue();
                    if (entry == null)
                    {
                        return;
                    }

                    if (LogService.Filter != null)
                    {
                        var ok = await LogService.Filter.IncludeLog(entry);
                        if (!ok)
                        {
                            continue;
                        }
                    }

                    await LogService.Log(entry);

                    if (LogQueue.Count == 0)
                    {
                        LogWaiter.WaitOne();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                LogWaiter.Reset();
                Interlocked.Exchange(ref wakeUpLock, 0);
            }
        }
    }
}