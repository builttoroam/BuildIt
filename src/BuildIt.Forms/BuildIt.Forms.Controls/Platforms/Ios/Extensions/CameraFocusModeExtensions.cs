using AVFoundation;

namespace BuildIt.Forms.Controls.Platforms.Ios.Extensions
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
        internal static AVCaptureFocusMode ToPlatformFocusMode(this CameraFocusMode focusMode)
        {
            switch (focusMode)
            {
                case CameraFocusMode.Auto:
                case CameraFocusMode.Manual:
                    return AVCaptureFocusMode.AutoFocus;
                case CameraFocusMode.Continuous:
                    return AVCaptureFocusMode.ContinuousAutoFocus;
            }

            return default(AVCaptureFocusMode);
        }

        /// <summary>
        /// Extension method that returns control specific focus mode based on the platform mode
        /// </summary>
        /// <param name="focusMode">Platform specific focus mode</param>
        /// <returns>Control specific focus mode</returns>
        internal static CameraFocusMode ToControlFocusMode(this AVCaptureFocusMode focusMode)
        {
            switch (focusMode)
            {
                case AVCaptureFocusMode.AutoFocus:                
                    return CameraFocusMode.Auto;
                case AVCaptureFocusMode.ContinuousAutoFocus:
                    return CameraFocusMode.Continuous;
            }

            return default(CameraFocusMode);
        }
    }
}