using BuildIt.Web.Utilities;

namespace BuildIt.Web.Models
{
    public class PushRegistration
    {
        public string RegistrationId { get; set; }

        public string Handle { get; set; }

        public PushPlatform Platform { get; set; }

        public string UserId { get; set; }
    }
}
