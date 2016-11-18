using System;
using BuildIt.Bot.Client.Impl.Droid.Models;
using BuildIt.Bot.Client.Models;

namespace BuildIt.Bot.Client.Impl.Droid.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class Settings
    {
        internal bool IsAppInForeground { get; set; }

        /// <summary>
        /// Already existing push notification registration
        /// </summary>
        public string RegistrationId { get; set; }

        /// <summary>
        /// All the necessary settings for push notifications to work
        /// </summary>
        public PushNotificationSettings PushNotificationSettings { get; set; }

        private static volatile Settings instance;
        private static object syncRoot = new Object();

        private Settings() { }

        /// <summary>
        /// 
        /// </summary>
        internal static Settings Instance
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