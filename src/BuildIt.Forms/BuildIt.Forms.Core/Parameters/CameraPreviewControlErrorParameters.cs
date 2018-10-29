namespace BuildIt.Forms.Parameters
{
    /// <summary>
    /// Provides information about the camera preview error
    /// </summary>
    /// <typeparam name="TData">Data object passed with the error</typeparam>
    public class CameraPreviewControlErrorParameters<TData>
    {
        public CameraPreviewControlErrorParameters(string[] errors, TData data, bool handled = false)
        {
            Errors = errors;
            Data = data;
            Handled = Handled;
        }

        /// <summary>
        /// Gets error messages
        /// </summary>
        public string[] Errors { get; }

        /// <summary>
        /// Gets error data
        /// </summary>
        public TData Data { get; }

        /// <summary>
        /// Gets a value indicating whether an error was handled
        /// </summary>
        public bool Handled { get; }
    }

    /// <summary>
    /// Provides information about the camera preview error
    /// </summary>
    public class CameraPreviewControlErrorParameters
    {
        public CameraPreviewControlErrorParameters(string[] errors, bool handled = false)
        {
            Errors = errors;
            Handled = Handled;
        }

        /// <summary>
        /// Gets error messages
        /// </summary>
        public string[] Errors { get; }

        /// <summary>
        /// Gets a value indicating whether an error was handled
        /// </summary>
        public bool Handled { get; }
    }
}
