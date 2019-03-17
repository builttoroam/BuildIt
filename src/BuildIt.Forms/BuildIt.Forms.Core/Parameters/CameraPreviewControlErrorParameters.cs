namespace BuildIt.Forms.Parameters
{
    /// <summary>
    /// Provides information about the camera preview error
    /// </summary>
    /// <typeparam name="TData">Data object passed with the error</typeparam>
    public class CameraPreviewControlErrorParameters<TData> : CameraPreviewControlErrorParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CameraPreviewControlErrorParameters"/> class.
        /// </summary>
        /// <param name="errors">errors to emit</param>
        /// <param name="data">the data to include relating to the error</param>
        /// <param name="handled">indicates whether the error has been handled or not</param>
        public CameraPreviewControlErrorParameters(string[] errors, TData data, bool handled = false) : base(errors, handled)
        {
            Data = data;
        }

        /// <summary>
        /// Gets error data
        /// </summary>
        public TData Data { get; }
    }

    /// <summary>
    /// Provides information about the camera preview error
    /// </summary>
    public class CameraPreviewControlErrorParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CameraPreviewControlErrorParameters"/> class.
        /// </summary>
        /// <param name="errors">errors to emit</param>
        /// <param name="handled">indicates whether the error has been handled or not</param>
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