using Android.Content;
using Android.Views;
using System;

namespace BuildIt.Forms.Controls.Platforms.Android
{
    public class DeviceOrientationListener : OrientationEventListener
    {
        private readonly Action orientationChangedAction;
        private int previousOriention = 0;

        public DeviceOrientationListener(Context context, Action orientationChangedAction)
            : base(context)
        {
            this.orientationChangedAction = orientationChangedAction;
        }

        public override void OnOrientationChanged(int orientation)
        {
            if (Math.Abs(orientation - previousOriention) >= 90)
            {
                orientationChangedAction?.Invoke();
            }
        }
    }
}