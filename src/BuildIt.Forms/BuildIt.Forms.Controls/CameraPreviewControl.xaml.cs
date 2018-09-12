﻿using System;
using System.Collections.Generic;
using System.Threading;
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
        private readonly SemaphoreSlim startCameraPreviewSemaphoreSlim = new SemaphoreSlim(1);
        private readonly SemaphoreSlim stopCameraPreviewSemaphoreSlim = new SemaphoreSlim(1);

        /// <summary>
        /// Informs the previewer as to which camera on the device should be used (if available)
        /// </summary>
        public static readonly BindableProperty PreferredCameraProperty =
            BindableProperty.Create(nameof(PreferredCamera), typeof(CameraFacing), typeof(CameraPreviewControl), CameraFacing.Back);

        /// <summary>
        /// Toggles enabling continuous autofocus
        /// </summary>
        public static readonly BindableProperty EnableContinuousAutoFocusProperty =
            BindableProperty.Create(nameof(EnableContinuousAutoFocus), typeof(bool), typeof(CameraPreviewControl), false);

        public static readonly BindableProperty AspectProperty =
            BindableProperty.Create(nameof(Aspect), typeof(Aspect), typeof(CameraPreviewControl), default(Aspect));

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
        /// Enumeration the available camera facings/positions
        /// </summary>
        public enum CameraFacing
        {
            /// <summary>
            /// An unspecified camera facing
            /// </summary>
            Unspecified,

            /// <summary>
            /// The camera located on the back of the device enclosure
            /// </summary>
            Back,

            /// <summary>
            /// The front-facing camera
            /// </summary>
            Front
        }

        public enum CameraStatus
        {
            /// <summary>
            /// Default state of the camera. Camera hasn't been interacted with
            /// </summary>
            None,

            /// <summary>
            /// Camera preview about to be started
            /// </summary>
            Starting,

            /// <summary>
            /// Camera preview has been started
            /// </summary>
            Started,

            /// <summary>
            /// Camera preview has been paused
            /// </summary>
            Paused,

            /// <summary>
            /// Camera preview has been stopped
            /// </summary>
            Stopped
        }

        /// <summary>
        /// Indicator of the current camera status
        /// </summary>
        public CameraStatus Status { get; internal set; }

        /// <summary>
        /// A callback for when a camera receives a frame
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
        /// Toggles continuous auto-focus for selected camera device
        /// </summary>
        public bool EnableContinuousAutoFocus
        {
            get => (bool)GetValue(EnableContinuousAutoFocusProperty);
            set => SetValue(EnableContinuousAutoFocusProperty, value);
        }

        public Aspect Aspect
        {
            get => (Aspect)GetValue(AspectProperty);
            set => SetValue(AspectProperty, value);
        }

        /// <summary>
        /// A delegate used by the native renderer implementation to start camera preview
        /// </summary>
        internal Func<Task> StartPreviewFunc { get; set; }

        /// <summary>
        /// A delegate used by the native renderer implementation to stop camera preview
        /// </summary>
        internal Func<Task> StopPreviewFunc { get; set; }

        /// <summary>
        /// A delegate used by the native renderer implementation to capture a frame of video to a file
        /// </summary>
        internal Func<bool, Task<string>> CaptureNativeFrameToFileFunc { get; set; }

        /// <summary>
        /// A delegate used by the native renderers to return the supported focus modes
        /// </summary>
        internal Func<IReadOnlyList<CameraFocusMode>> RetrieveSupportedFocusModesFunc { get; set; }

        /// <summary>
        /// A delegate used by the native renderers to return the available cameras
        /// </summary>
        internal Func<Task<IReadOnlyList<ICamera>>> RetrieveCamerasFunc { get; set; }

        /// <summary>
        /// Start camera preview
        /// </summary>
        /// <returns></returns>
        public async Task StartPreview()
        {
            if (StartPreviewFunc == null || Status == CameraStatus.Started || Status == CameraStatus.Starting)
            {
                return;
            }

            if (await startCameraPreviewSemaphoreSlim.WaitAsync(0))
            {
                Status = CameraStatus.Starting;

                await StartPreviewFunc.Invoke();
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

            if (await stopCameraPreviewSemaphoreSlim.WaitAsync(0))
            {
                await StopPreviewFunc.Invoke();
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

        internal async Task OnMediaFrameArrived(MediaFrame mediaFrame)
        {
            if (MediaFrameArrived == null)
            {
                return;
            }

            await MediaFrameArrived(mediaFrame);
        }

        internal void RaiseErrorOpeningCamera()
        {
            Status = CameraStatus.Stopped;
            ErrorOpeningCamera?.Invoke(this, EventArgs.Empty);
        }
    }
}