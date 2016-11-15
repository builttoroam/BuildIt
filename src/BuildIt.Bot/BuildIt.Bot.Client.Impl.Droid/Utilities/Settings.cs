using System;
using BuildIt.Bot.Client.Impl.Droid.Models;
using BuildIt.Bot.Client.Models;

namespace BuildIt.Bot.Client.Impl.Droid.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Settings
    {
        internal bool IsAppInForeground { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public EndpointRouteDetails EndpointRouteDetails { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Type MainActivityType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PushNotificationDetails PushNotificationDetails { get; set; } = new PushNotificationDetails();

        /// <summary>
        /// 
        /// </summary>
        public string RegistrationId { get; set; }

        private static volatile Settings instance;
        private static object syncRoot = new Object();

        private Settings() { }

        /// <summary>
        /// 
        /// </summary>
        public static Settings Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new Settings();
                        }
                    }
                }

                return instance;
            }
        }        
    }
}