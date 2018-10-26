﻿namespace BuildIt.Forms
{
    /// <summary>
    ///  The available focus modes
    /// </summary>
    public enum CameraFocusMode
    {
        /// <summary>
        /// An unspecified focus mode
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// The camera will manually focus on a point/region
        /// </summary>
        Manual = 1,

        /// <summary>
        /// The camera will automatically focus once
        /// </summary>
        Auto = 2,

        /// <summary>
        /// The camera will continuously try to focus
        /// </summary>
        Continuous = 3
    }
}