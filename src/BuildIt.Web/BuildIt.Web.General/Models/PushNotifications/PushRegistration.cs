using System.Collections.Generic;
using BuildIt.Web.Utilities;

namespace BuildIt.Web.Models.PushNotifications
{
    /// <summary>
    /// 
    /// </summary>
    public class PushRegistration
    {
        /// <summary>
        /// 
        /// </summary>
        public string RegistrationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PushPlatform Platform { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Tags { get; set; }
    }
}
