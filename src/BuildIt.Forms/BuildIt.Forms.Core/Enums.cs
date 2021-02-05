namespace BuildIt.Forms
{
    /// <summary>
    ///  The available focus modes.
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
        Continuous = 3,
    }

    public enum StatefulControlStates
    {
        /// <summary>
        /// State that the stateful control starts with (no state)
        /// </summary>
        Default,

        /// <summary>
        /// State that indicates that the control is currently processing data
        /// </summary>
        Loading,

        /// <summary>
        /// State that indicates that the control has no data to display
        /// </summary>
        Empty,

        /// <summary>
        /// State that indicates that the control had some issues with loading data
        /// </summary>
        LoadingError,

        /// <summary>
        /// State that indicates that the control has successfully loaded data
        /// </summary>
        Loaded,
    }
}