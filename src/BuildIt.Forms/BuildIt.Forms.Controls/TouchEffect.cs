using Xamarin.Forms;

namespace BuildIt.Forms.Controls
{
    public class TouchEffect : RoutingEffect
    {
        public event TouchActionEventHandler TouchAction;

        public TouchEffect() : base("BuildIt.TouchEffect")
        {
        }

        public bool Capture { set; get; }

        public void OnTouchAction(Element element, TouchActionEventArgs args)
        {
            TouchAction?.Invoke(element, args);
        }
    }
}
