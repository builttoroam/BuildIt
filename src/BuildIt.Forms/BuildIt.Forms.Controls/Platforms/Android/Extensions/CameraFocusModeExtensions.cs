using System.Collections.Generic;
using System.Linq;
using Android.Hardware.Camera2;

namespace BuildIt.Forms.Controls.Platforms.Android.Extensions
{
    /// <summary>
    /// Extension methods for the CameraFocusModeExtensions
    /// </summary>
    internal static class CameraFocusModeExtensions
    {
        /// <summary>
        /// Extension method that returns platform specific focus mode based on the control mode
        /// </summary>
        /// <param name="focusMode">Control specific focus mode</param>
        /// <returns>Platform specific focus mode</returns>
        internal static ControlAFMode ToPlatformFocusMode(this CameraFocusMode focusMode)
        {
            switch (focusMode)
            {
                case CameraFocusMode.Auto:
                    return ControlAFMode.Auto;
                case CameraFocusMode.Continuous:
                    // TODO distinguish between Video and Picture?
                    return ControlAFMode.ContinuousPicture;
                case CameraFocusMode.Manual:
                    return ControlAFMode.Off;
            }

            return default(ControlAFMode);
        }

        /// <summary>
        /// Extension method that returns control specific focus mode based on the platform mode
        /// </summary>
        /// <param name="focusMode">Platform specific focus mode</param>
        /// <returns>Control specific focus mode</returns>
        internal static CameraFocusMode ToControlFocusMode(this ControlAFMode focusMode)
        {
            switch (focusMode)
            {
                case ControlAFMode.Auto:
                    return CameraFocusMode.Auto;
                case ControlAFMode.ContinuousPicture:
                case ControlAFMode.ContinuousVideo:
                    return CameraFocusMode.Continuous;
                case ControlAFMode.Off:
                    return CameraFocusMode.Manual;
            }

            return default(CameraFocusMode);
        }
    }
}
