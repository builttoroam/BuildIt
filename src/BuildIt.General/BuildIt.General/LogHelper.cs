using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using BuildIt.ServiceLocation;
using Newtonsoft.Json;

namespace BuildIt
{
    /// <summary>
    /// Helper class that simplifies writing log information
    /// </summary>
    public static class LogHelper
    {
        private static ILogService logService;
        private static bool hasLookedForLogService;

        private static ILogService LogService
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
                    return logService ?? (logService = ServiceLocator.Current.GetInstance<ILogService>());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error retrieving ILogService implementation: " + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// Logs information about an entity
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to write</typeparam>
        /// <param name="entity">The entity to write (serialized)</param>
        /// <param name="caller">The name of the calling method</param>
        public static void Log<TEntity>(this TEntity entity, [CallerMemberName] string caller = null)
        {
            var json = JsonConvert.SerializeObject(entity);

            // ReSharper disable once ExplicitCallerInfoArgument // Ignore that argument can be null
            Log(typeof(TEntity).Name + ": " + json, caller);
        }

        /// <summary>
        /// Log out a message
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="caller">The name of the calling method</param>
        public static void Log(this string message, [CallerMemberName] string caller = null)
        {
            try
            {
                InternalWriteLog("[" + caller + "] " + message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Logs an exception
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="message">The message (optional) to log</param>
        /// <param name="caller">The calling method</param>
        public static void LogException(this Exception ex, string message = null, [CallerMemberName] string caller = null)
        {
            try
            {
                InternalWriteException(caller + ": " + message, ex);
            }
            catch (Exception ext)
            {
                Debug.WriteLine(ext.Message);
            }
        }

    private static void InternalWriteLog(string message)
        {
            try
            {
                Debug.WriteLine(message);
                LogService?.Debug(message);
            }
            catch (Exception ext)
            {
                Debug.WriteLine(ext.Message);
            }
        }

        private static void InternalWriteException(string message, Exception ex)
        {
            try
            {
                Debug.WriteLine($"Exception ({message}): {ex.Message}");
                LogService?.Exception(message, ex);
            }
            catch (Exception ext)
            {
                Debug.WriteLine("Exception: " + ext.Message);
            }
        }
    }
}