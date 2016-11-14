using System;
using BuildIt.Bot.Client.Impl.Droid.Models;
using BuildIt.Bot.Client.Models;

namespace BuildIt.Bot.Client.Impl.Droid.Utilities
{
    public sealed class Settings
    {
        internal bool IsAppInForeground { get; set; }

        public EndpointRouteDetails EndpointRouteDetails { get; set; }

        public Type MainActivityType { get; set; }

        public PushNotificationDetails PushNotificationDetails { get; set; } = new PushNotificationDetails();

        private static volatile Settings instance;
        private static object syncRoot = new Object();

        private Settings() { }

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