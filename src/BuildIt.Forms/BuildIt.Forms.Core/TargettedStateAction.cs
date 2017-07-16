using Xamarin.Forms;

namespace BuildIt.Forms.Core
{
    public class TargettedStateAction
    {
        public string Target { get; set; }

        public Element Element { get; set; }

        public string Property { get; set; }

        public int Duration { get; set; }
    }
}