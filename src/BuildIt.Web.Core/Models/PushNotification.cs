namespace BuildIt.Web.Core.Models
{
    public class PushNotification
    {
        public string Title { get; set; }
        public string Body { get; set; }

        public PushPlatform Platforms { get; set; }
    }
}
