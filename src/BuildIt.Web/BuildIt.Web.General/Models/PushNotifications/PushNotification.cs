using BuildIt.Web.Utilities;

namespace BuildIt.Web.Models.PushNotifications
{
    /// <summary>
    /// 
    /// </summary>
    public class PushNotification
    {
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PushPlatform Platforms { get; set; }
    }
}
