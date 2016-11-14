#if NET46

using System;
using BuildIt.Web.Utilities;

namespace BuildIt.Bot.Api.Models
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PushNotificationDetails
    {
        /// <summary>
        /// 
        /// </summary>
        public string PushNotificationTitle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PushPlatform SupportedPushPlatforms { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PushNotificationLaunchArgument { get; set; }
    }
}

#endif