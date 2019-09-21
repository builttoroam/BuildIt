using Windows.Media.Devices;

namespace BuildIt.Forms.Controls.Platforms.Uap.Extensions
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
        internal static FocusMode ToPlatformFocusMode(this CameraFocusMode focusMode)
        {
            switch (focusMode)
            {
                case CameraFocusMode.Auto:
                    return FocusMode.Auto;

                case CameraFocusMode.Continuous:
                    return FocusMode.Continuous;

                case CameraFocusMode.Manual:
                    return FocusMode.Manual;
            }

            return default(FocusMode);
        }

        /// <summary>
        /// Extension method that returns control specific focus mode based on the platform mode.
        /// </summary>
        /// <param name="focusMode">Platform specific focus mode.</param>
        /// <returns>Control specific focus mode.</returns>
        internal static CameraFocusMode ToControlFocusMode(this FocusMode focusMode)
        {
            switch (focusMode)
            {
                case FocusMode.Auto:
                    return CameraFocusMode.Auto;

                case FocusMode.Continuous:
                    return CameraFocusMode.Continuous;

                case FocusMode.Manual:
                    return CameraFocusMode.Manual;
            }

            return default(CameraFocusMode);
        }
    }
}