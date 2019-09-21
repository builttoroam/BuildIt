using Android.Hardware.Camera2;
#pragma warning disable CS0618 // Type or member is obsolete

namespace BuildIt.Forms.Controls.Platforms.Android.Extensions
{
    /// <summary>
    /// Extension methods for the CameraFocusModeExtensions.
    /// </summary>
    internal static class CameraFocusModeExtensions
    {
        /// <summary>
        /// Extension method that returns platform specific focus mode based on the control mode.
        /// </summary>
        /// <param name="focusMode">Control specific focus mode.</param>
        /// <returns>Platform specific focus mode.</returns>
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
        /// Extension method that returns control specific focus mode based on the platform mode.
        /// </summary>
        /// <param name="focusMode">Platform specific focus mode.</param>
        /// <returns>Control specific focus mode.</returns>
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

        /// <summary>
        /// Extension method that returns control specific focus mode based on the platform mode.
        /// </summary>
        /// <param name="focusMode">Platform specific focus mode.</param>
        /// <returns>Control specific focus mode.</returns>
        internal static CameraFocusMode ToPlatformFocusMode(this string focusMode)
        {
            switch (focusMode)
            {
                case global::Android.Hardware.Camera.Parameters.FocusModeAuto:
                    return CameraFocusMode.Auto;

                case global::Android.Hardware.Camera.Parameters.FocusModeContinuousPicture:
                    return CameraFocusMode.Continuous;

                case global::Android.Hardware.Camera.Parameters.FocusModeFixed:
                    return CameraFocusMode.Manual;
            }

            return default(CameraFocusMode);
        }
    }
}