using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BuildIt.Forms.Controls
{
    /// <summary>
    /// A simple controled inheritng from <see cref="Frame"/> which shows a live preview of the camera. Defaults to rear/first-available camera
    /// iOS: Requires 'NSCameraUsageDescription' in your info.plist. Android: Requires 'android.permission.CAMERA' in the app manifest. UWP: Requires 'Webcam' and 'Microphone' capabilities
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CameraPreviewControl
    {
        /// <summary>
        /// Informs the previewer as to which camera on the device should be used (if available)
        /// </summary>
        public static readonly BindableProperty PreferredCameraProperty =
            BindableProperty.Create(nameof(PreferredCamera), typeof(CameraPreference), typeof(CameraPreviewControl), CameraPreference.Back);

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraPreviewControl"/> class.
        /// </summary>
        public CameraPreviewControl()
        {
            InitializeComponent();
        }

        internal Func<bool, Task> EnableAutoContinuousAutoFocus { get; set; }

        /// <summary>
        /// A delegate type used by the native renderer implementation to capture a frame of video to a file. Android: Requires 'android.permission.WRITE_EXTERNAL_STORAGE' in manifest
        /// </summary>
        /// <param name="saveToPhotosLibrary">Whether or not to add the file to the device's photo library.
        /// **If Saving to Photos Library** iOS: Requires `NSPhotoLibraryUsageDescription' in info.plist. UWP: Requires 'Pictures Library' capability</param>
        /// <returns>The path to the saved photo file</returns>
        internal delegate Task<string> CaptureNativeFrameToFile(bool saveToPhotosLibrary);

        /// <summary>
        /// Enumeration specifying which camera should be used
        /// </summary>
        public enum CameraPreference
        {
            /// <summary>
            /// Prefer the camera located on the back of the device enclosure
            /// </summary>
            Back,

            /// <summary>
            /// Prefer the front-facing camera
            /// </summary>
            Front
        }

        /// <summary>
        /// Gets or sets the preferred camera to be used for the preview
        /// </summary>
        public CameraPreference PreferredCamera
        {
            get => (CameraPreference)GetValue(PreferredCameraProperty);
            set => SetValue(PreferredCameraProperty, value);
        }

        /// <summary>
        /// Gets or sets the native implementation of the frame capture method
        /// </summary>
        internal CaptureNativeFrameToFile CaptureNativeFrameToFileDelegate { get; set; }

        /// <summary>
        /// Captures the most current video frame to a photo and saves it to local storage. Android: Requires 'android.permission.WRITE_EXTERNAL_STORAGE' in manifest
        /// </summary>
        /// <param name="saveToPhotosLibrary">Whether or not to add the file to the device's photo library.
        /// **If Saving to Photos Library** iOS: Requires `NSPhotoLibraryUsageDescription' in info.plist. UWP: Requires 'Pictures Library' capability</param>
        /// <returns>The path to the saved photo file</returns>
        public Task<string> CaptureFrameToFile(bool saveToPhotosLibrary)
        {
            return CaptureNativeFrameToFileDelegate?.Invoke(saveToPhotosLibrary);
        }


    }
}