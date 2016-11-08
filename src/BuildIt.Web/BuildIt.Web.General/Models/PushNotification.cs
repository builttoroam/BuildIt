using BuildIt.Web.Utilities;

namespace BuildIt.Web.Models
{
    public class PushNotification
    {
        public string Title { get; set; }
        public string Body { get; set; }

        public PushPlatform Platforms { get; set; }
    }
}
