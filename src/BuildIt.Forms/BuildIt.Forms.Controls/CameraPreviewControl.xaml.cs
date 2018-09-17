using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using BuildIt.Forms.Controls.Common;
using BuildIt.Forms.Controls.Interfaces;
using BuildIt.Forms.Parameters;
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
        public static readonly BindableProperty PreferredCameraProperty = BindableProperty.Create(nameof(PreferredCamera), typeof(CameraFacing), typeof(CameraPreviewControl), CameraFacing.Back);

        /// <summary>
        /// Gets or sets the focus mode for the camera preview
        /// </summary>
        public static readonly BindableProperty FocusModeProperty = BindableProperty.Create(nameof(FocusMode), typeof(CameraFocusMode), typeof(CameraPreviewControl), CameraFocusMode.Continuous, propertyChanged: FocusModePropertyChanged);

        /// <summary>
        /// TODO Add summary
        /// </summary>
        public static readonly BindableProperty AspectProperty = BindableProperty.Create(nameof(Aspect), typeof(Aspect), typeof(CameraPreviewControl), default(Aspect));

        /// <summary>
        /// Executed when there's an error in the camera preview control
        /// </summary>
        public static readonly BindableProperty ErrorCommandProperty = BindableProperty.Create(nameof(ErrorCommand), typeof(ICommand), typeof(CameraPreviewControl), null);

        private readonly SemaphoreSlim startStopCameraPreviewSemaphoreSlim = new SemaphoreSlim(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraPreviewControl"/> class.
        /// </summary>
        public CameraPreviewControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Raised when there's an error opening the camera
        /// </summary>
        public event EventHandler ErrorOpeningCamera;

        /// <summary>
        /// Gets indicator of the current camera status
        /// </summary>
        public CameraStatus Status { get; internal set; }

        /// <summary>
        /// Gets indicator of the current error camera status
        /// </summary>
        public CameraErrorCode ErrorCode { get; internal set; }

        /// <summary>
        /// Gets or sets a callback for when a camera receives a frame
        /// </summary>
        public Func<MediaFrame, Task> MediaFrameArrived { get; set; }

        /// <summary>
        /// Gets or sets the preferred camera to be used for the preview
        /// </summary>
        public CameraFacing PreferredCamera
        {
            get => (CameraFacing)GetValue(PreferredCameraProperty);
            set => SetValue(PreferredCameraProperty, value);
        }

        /// <summary>
        /// Gets or sets the focus mode for the camera preview
        /// </summary>
        public CameraFocusMode FocusMode
        {
            get => (CameraFocusMode)GetValue(FocusModeProperty);
            set => SetValue(FocusModeProperty, value);
        }

        /// <summary>
        /// Gets or sets // TODO Add summary
        /// </summary>
        public Aspect Aspect
        {
            get => (Aspect)GetValue(AspectProperty);
            set => SetValue(AspectProperty, value);
        }

        /// <summary>
        /// Gets or sets the command that will be executed every time the camera preview control got into an error state
        ///
        /// NOTE: When binding it in XAML, make sure that it's defined at the very top of the bindable properties
        /// </summary>
        public ICommand ErrorCommand
        {
            get => (ICommand)GetValue(ErrorCommandProperty);
            set => SetValue(ErrorCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the delegate for the logic to request camera permissions
        ///
        /// NOTE: The invocation of this method happens when StartPreview has been called
        /// </summary>
        public Func<Task<bool>> RequestCameraPermissionsCallback { get; set; }

        /// <summary>
        /// Gets or sets a delegate used by the native renderer implementation to start camera preview
        /// </summary>
        internal Func<Task> StartPreviewFunc { get; set; }

        /// <summary>
        /// Gets or sets a delegate used by the native renderer implementation to stop camera preview
        /// </summary>
        internal Func<ICameraPreviewStopParameters, Task> StopPreviewFunc { get; set; }

        /// <summary>
        /// Gets or sets a delegate used by the native renderer implementation to capture a frame of video to a file
        /// </summary>
        internal Func<bool, Task<string>> CaptureNativeFrameToFileFunc { get; set; }

        /// <summary>
        /// Gets or sets a delegate used by the native renderers to return the supported focus modes
        /// </summary>
        internal Func<IReadOnlyList<CameraFocusMode>> RetrieveSupportedFocusModesFunc { get; set; }

        /// <summary>
        /// Gets or sets A delegate used by the native renderers to return the available cameras
        /// </summary>
        internal Func<Task<IReadOnlyList<ICamera>>> RetrieveCamerasFunc { get; set; }

        /// <summary>
        /// Start camera preview
        /// </summary>
        /// <returns></returns>
        public async Task StartPreview()
        {
            if (StartPreviewFunc == null)
            {
                return;
            }

            if (await startStopCameraPreviewSemaphoreSlim.WaitAsync(0))
            {
                try
                {
                    if (Status == CameraStatus.Started || Status == CameraStatus.Starting)
                    {
                        return;
                    }

                    // If the callback hasn't been provided assume that we have all of the permissions
                    // If it turns out that the app doesn't have permissions, we should handle it gracefully and return with according info
                    var hasCameraPermissions = true;
                    if (RequestCameraPermissionsCallback != null)
                    {
                        hasCameraPermissions = await RequestCameraPermissionsCallback.Invoke();
                    }

                    if (!hasCameraPermissions)
                    {
                        Status = CameraStatus.Error;
                        ErrorCode = CameraErrorCode.PermissionsNotGranted;
                        return;
                    }

                    Status = CameraStatus.Starting;

                    await StartPreviewFunc.Invoke();
                }
                finally
                {
                    startStopCameraPreviewSemaphoreSlim.Release();
                }
            }
        }

        /// <summary>
        /// Stop camera preview
        /// </summary>
        /// <returns></returns>
        public async Task StopPreview()
        {
            if (StopPreviewFunc == null || Status == CameraStatus.Stopped || Status == CameraStatus.None)
            {
                return;
            }

            if (await startStopCameraPreviewSemaphoreSlim.WaitAsync(0))
            {
                try
                {
                    await StopPreviewFunc.Invoke(null);
                }
                finally
                {
                    startStopCameraPreviewSemaphoreSlim.Release();
                }
            }
        }

        /// <summary>
        /// Captures the most current video frame to a photo and saves it to local storage. Android: Requires 'android.permission.WRITE_EXTERNAL_STORAGE' in manifest
        /// </summary>
        /// <param name="saveToPhotosLibrary">Whether or not to add the file to the device's photo library.
        /// **If Saving to Photos Library** iOS: Requires `NSPhotoLibraryUsageDescription' in info.plist. UWP: Requires 'Pictures Library' capability</param>
        /// <returns>The path to the saved photo file</returns>
        public async Task<string> CaptureFrameToFile(bool saveToPhotosLibrary)
        {
            if (CaptureNativeFrameToFileFunc == null)
            {
                return null;
            }

            return await CaptureNativeFrameToFileFunc(saveToPhotosLibrary);
        }

        /// <summary>
        /// Retrieves the focus modes supported by the currently selected camera
        /// </summary>
        /// <returns>The supported focus modes</returns>
        public IReadOnlyList<CameraFocusMode> RetrieveSupportedFocusModes()
        {
            if (RetrieveSupportedFocusModesFunc == null)
            {
                return new List<CameraFocusMode>();
            }

            return RetrieveSupportedFocusModesFunc();
        }

        /// <summary>
        /// Method that sets focus to a specific distance
        /// </summary>
        /// <param name="focusDistance">Focus distance (MUST be between 0.0 and 1.0)</param>
        /// <returns>Indicator if focusing succeeded</returns>
        public async Task<bool> Focus(double focusDistance)
        {
            if (Status != CameraStatus.Started)
            {
                return false;
            }

            var supportedFocusModes = RetrieveSupportedFocusModes();
            if (supportedFocusModes.Any(m => m == CameraFocusMode.Manual))
            {
                // TODO Implement
                return true;
            }

            return false;
        }

        /// <summary>
        /// Retrieves the available camera facings
        /// </summary>
        /// <returns>The available camera facings</returns>
        public async Task<IReadOnlyList<ICamera>> RetrieveCamerasAsync()
        {
            if (RetrieveCamerasFunc == null)
            {
                return new List<ICamera>();
            }

            return await RetrieveCamerasFunc();
        }

        /// <summary>
        /// TODO Add summary
        /// </summary>
        /// <param name="mediaFrame">MediaFrame</param>
        /// <returns></returns>
        internal async Task OnMediaFrameArrived(MediaFrame mediaFrame)
        {
            if (MediaFrameArrived == null)
            {
                return;
            }

            await MediaFrameArrived(mediaFrame);
        }

        /// <summary>
        /// Raises an error when something wrong happened when opening a camera
        /// </summary>
        internal void RaiseErrorOpeningCamera()
        {
            Status = CameraStatus.Stopped;
            ErrorOpeningCamera?.Invoke(this, EventArgs.Empty);
        }

        private static void FocusModePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var cameraPreviewControl = bindable as CameraPreviewControl;
            if (cameraPreviewControl == null)
            {
                return;
            }

            var value = (CameraFocusMode)newvalue;
            var supportedFocusModes = cameraPreviewControl.RetrieveSupportedFocusModes();
            if (supportedFocusModes.Any(m => m == value))
            {
                cameraPreviewControl.FocusMode = value;
            }
            else
            {
                var fallbackFocusMode = supportedFocusModes.OrderBy(m => m).LastOrDefault();
                cameraPreviewControl.FocusMode = fallbackFocusMode;
                cameraPreviewControl.ErrorCommand?.Execute(new CameraPreviewControlErrorParameters<CameraFocusMode>(string.Format(Strings.Errors.UnsupportedFocusModeFormat, value, fallbackFocusMode), fallbackFocusMode, true));
            }
        }
    }
}