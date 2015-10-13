using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;

namespace BuildIt
{
    public static class LogHelper
    {
        public static void Log<TEntity>(this TEntity entity, [CallerMemberName] string caller = null)
        {
            var json = JsonConvert.SerializeObject(entity);
            // ReSharper disable once ExplicitCallerInfoArgument // Ignore that argument can be null
            Log(typeof (TEntity).Name + ": " + json, caller);
        }

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

        public static void LogException(this Exception ex, string message = null,
            [CallerMemberName] string caller = null)
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


        private static ILogService logService;
        private static bool hasLookedForLogService;

        private static ILogService LogService
        {
            get
            {
                try
                {
                    if (hasLookedForLogService) return logService;
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