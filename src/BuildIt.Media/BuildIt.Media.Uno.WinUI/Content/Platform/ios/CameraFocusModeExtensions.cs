﻿using AVFoundation;
using Windows.Media.Devices;

namespace BuildIt.Media.Uno.WinUI
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
        internal static AVCaptureFocusMode ToPlatformFocusMode(this FocusMode focusMode)
        {
            switch (focusMode)
            {
                case FocusMode.Auto:
                case FocusMode.Manual:
                    return AVCaptureFocusMode.AutoFocus;

                case FocusMode.Continuous:
                    return AVCaptureFocusMode.ContinuousAutoFocus;
            }

            return default(AVCaptureFocusMode);
        }

        /// <summary>
        /// Extension method that returns control specific focus mode based on the platform mode.
        /// </summary>
        /// <param name="focusMode">Platform specific focus mode.</param>
        /// <returns>Control specific focus mode.</returns>
        internal static FocusMode ToControlFocusMode(this AVCaptureFocusMode focusMode)
        {
            switch (focusMode)
            {
                case AVCaptureFocusMode.AutoFocus:
                    return FocusMode.Auto;

                case AVCaptureFocusMode.ContinuousAutoFocus:
                    return FocusMode.Continuous;
            }

            return default(FocusMode);
        }
    }
}