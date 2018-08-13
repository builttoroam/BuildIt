using BuildIt.Forms.Controls;
using BuildIt.Forms.Controls.Platforms.Uap;
using BuildIt.Forms.Controls.Platforms.Uap.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using Application = Windows.UI.Xaml.Application;
using FlowDirection = Windows.UI.Xaml.FlowDirection;
using Frame = Xamarin.Forms.Frame;
using Grid = Windows.UI.Xaml.Controls.Grid;
using Panel = Windows.Devices.Enumeration.Panel;

[assembly: ExportRenderer(typeof(CameraPreviewControl), typeof(CameraPreviewControlRenderer))]
namespace BuildIt.Forms.Controls.Platforms.Uap
{
    /// <summary>
    /// Custom Renderer for <see cref="CameraPreviewControl"/>
    /// </summary>
    public class CameraPreviewControlRenderer : FrameRenderer
    {
        // Rotation metadata to apply to preview stream (https://msdn.microsoft.com/en-us/library/windows/apps/xaml/hh868174.aspx)
        private static readonly Guid RotationKey = new Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1"); // (MF_MT_VIDEO_ROTATION)

        // Prevent the screen from sleeping while the camera is running
        private readonly DisplayRequest displayRequest = new DisplayRequest();

        private MediaCapture mediaCapture;
        private CaptureElement captureElement;

        private CameraRotationHelper rotationHelper;
        private Application app;

        private bool isSuspending;
        private bool isInitialized;
        private bool isPreviewing;
        private bool isUiActive;
        private bool isActivePage;
        private bool mirroringPreview;
        private bool externalCamera;
        private Task setupTask = Task.CompletedTask;

        private CameraPreviewControl cameraPreviewControl;
        private MediaFrameReader mediaFrameReader;
        private bool processing;

        /// <inheritdoc />
        protected override async void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            var cpc = Element as CameraPreviewControl;
            if (cpc == null)
            {
                return;
            }

            cameraPreviewControl = cpc;
            cameraPreviewControl.CaptureNativeFrameToFileFunc = CapturePhotoToFile;
            cameraPreviewControl.RetrieveSupportedFocusModesFunc = RetrieveSupportedFocusModes;
            cameraPreviewControl.RetrieveCamerasFunc = RetrieveCamerasAsync;
            SetupUserInterface();
            isActivePage = true;
            await SetupBasedOnStateAsync();

            if (e.OldElement != null)
            {
                return;
            }

            try
            {
                app = Application.Current;
                app.Suspending += OnAppSuspending;
                app.Resuming += OnAppResuming;
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
        }

        /// <summary>
        /// Captures the most current video frame to a photo and saves it to local storage
        /// </summary>
        /// <param name="saveToPhotosLibrary">Whether or not to add the file to the device's photo library.
        /// **If Saving to Photos Library** Requires 'Pictures Library' capability</param>
        /// <returns>The path to the saved photo file</returns>
        protected virtual async Task<string> CapturePhotoToFile(bool saveToPhotosLibrary)
        {
            try
            {
                var stream = new InMemoryRandomAccessStream();
                await mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);

                StorageFolder captureRoot = null;
                if (saveToPhotosLibrary)
                {
                    var picturesLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
                    captureRoot = picturesLibrary.SaveFolder;
                }

                captureRoot = captureRoot ?? ApplicationData.Current.LocalFolder;
                var captureFolder = await captureRoot.CreateFolderAsync(Path.Combine("VideoCapture", DateTime.Now.ToString("yyyy-MM-dd")), CreationCollisionOption.OpenIfExists);
                var fileCount = (await captureFolder.GetFilesAsync()).Count;
                var file = await captureFolder.CreateFileAsync($"{fileCount}.jpg", CreationCollisionOption.GenerateUniqueName);
                var orientation = CameraRotationHelper.ConvertSimpleOrientationToPhotoOrientation(rotationHelper.GetCameraCaptureOrientation());
                await ReencodeAndSavePhotoAsync(stream, file, orientation);

                return file.Path;
            }
            catch (Exception ex)
            {
                ex.LogError();
                return ex.ToString();
            }
        }

        /// <inheritdoc />
        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
            {
                await SetupBasedOnStateAsync();
            }

            if (e.PropertyName == CameraPreviewControl.PreferredCameraProperty.PropertyName &&
                isUiActive)
            {
                // restart the previewer so that it can pick up the correct camera preference
                await CleanupCameraAsync();
                await InitializeCameraAsync();
            }

            if (e.PropertyName == CameraPreviewControl.EnableContinuousAutoFocusProperty.PropertyName)
            {
                await EnableContinuousAutoFocusAsync(cameraPreviewControl.EnableContinuousAutoFocus);
            }
        }

        private static async Task ReencodeAndSavePhotoAsync(IRandomAccessStream stream, StorageFile file, PhotoOrientation orientation)
        {
            using (var inputStream = stream)
            {
                var decoder = await BitmapDecoder.CreateAsync(inputStream);
                using (var outputStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateForTranscodingAsync(outputStream, decoder);
                    var properties = new BitmapPropertySet { { "System.Photo.Orientation", new BitmapTypedValue(orientation, PropertyType.UInt16) } };
                    await encoder.BitmapProperties.SetPropertiesAsync(properties);
                    await encoder.FlushAsync();
                }
            }
        }

        private static async Task<DeviceInformation> FindCameraDeviceByPanelAsync(Panel desiredPanel)
        {
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            var desiredDevice = allVideoDevices.FirstOrDefault(d => d.EnclosureLocation != null && d.EnclosureLocation.Panel == desiredPanel);
            return desiredDevice ?? allVideoDevices.FirstOrDefault();
        }

        private static CameraPreviewControl.CameraFacing ToCameraFacing(Panel panel)
        {
            switch (panel)
            {
                case Panel.Front:
                    return CameraPreviewControl.CameraFacing.Front;

                case Panel.Back:
                    return CameraPreviewControl.CameraFacing.Back;

                default:
                    return CameraPreviewControl.CameraFacing.Unspecified;
            }
        }

        private async Task SetupBasedOnStateAsync()
        {
            while (!setupTask.IsCompleted)
            {
                await setupTask;
            }

            var wantUiActive = isActivePage && Element.IsVisible && !isSuspending;
            if (isUiActive != wantUiActive)
            {
                isUiActive = wantUiActive;

                async Task SetupAsync()
                {
                    if (wantUiActive)
                    {
                        await InitializeCameraAsync();
                    }
                    else
                    {
                        await CleanupCameraAsync();
                    }
                }

                setupTask = SetupAsync();
            }

            await setupTask;
        }

        private void SetupUserInterface()
        {
            captureElement = new CaptureElement
            {
                Stretch = Stretch.UniformToFill
            };

            var grid = new Grid();
            grid.Children.Add(captureElement);

            Element.Content = grid.ToView();
        }

        private async Task InitializeCameraAsync()
        {
            if (mediaCapture == null)
            {
                var desiredPanel = cameraPreviewControl.PreferredCamera == CameraPreviewControl.CameraFacing.Back
                    ? Panel.Back
                    : Panel.Front;

                // Attempt to get the back camera if one is available, but use any camera device if not
                var cameraDevice = await FindCameraDeviceByPanelAsync(desiredPanel);

                if (cameraDevice == null)
                {
                    Debug.WriteLine("No camera device found.");
                    return;
                }

                var frameSourceGroups = await MediaFrameSourceGroup.FindAllAsync();
                var selectedGroupObjects = frameSourceGroups.Select(group =>
                   new
                   {
                       sourceGroup = group,
                       colorSourceInfo = group.SourceInfos.FirstOrDefault((sourceInfo) =>
                       {
                           return sourceInfo.SourceKind == MediaFrameSourceKind.Color;
                       })
                   }).Where(t => t.colorSourceInfo != null)
                   .FirstOrDefault();

                MediaFrameSourceGroup selectedGroup = selectedGroupObjects?.sourceGroup;
                MediaFrameSourceInfo colorSourceInfo = selectedGroupObjects?.colorSourceInfo;
                mediaCapture = new MediaCapture();
                var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id, SourceGroup = selectedGroup, StreamingCaptureMode = StreamingCaptureMode.Video };
                try
                {
                    await mediaCapture.InitializeAsync(settings);
                    var colorFrameSource = mediaCapture.FrameSources[colorSourceInfo.Id];
                    var preferredFormat = colorFrameSource.SupportedFormats.FirstOrDefault(f => f.Subtype == MediaEncodingSubtypes.Argb32);
                    if (preferredFormat != null)
                    {
                        await colorFrameSource.SetFormatAsync(preferredFormat);
                    }

                    mediaFrameReader = await mediaCapture.CreateFrameReaderAsync(colorFrameSource, MediaEncodingSubtypes.Argb32);
                    mediaFrameReader.FrameArrived += MediaFrameReader_FrameArrived;
                    await mediaFrameReader.StartAsync();
                    isInitialized = true;
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.WriteLine("The app was denied access to the camera.");
                }

                // If initialization succeeded, start the preview
                if (isInitialized)
                {
                    // Figure out where the camera is located
                    if (cameraDevice.EnclosureLocation == null || cameraDevice.EnclosureLocation.Panel == Panel.Unknown)
                    {
                        // No information on the location of the camera, assume it's an external camera, not integrated on the device
                        externalCamera = true;
                    }
                    else
                    {
                        // Camera is fixed on the device
                        externalCamera = false;

                        // Only mirror the preview if the camera is on the front panel
                        mirroringPreview = cameraDevice.EnclosureLocation.Panel == Panel.Front;
                    }

                    rotationHelper = new CameraRotationHelper(cameraDevice.EnclosureLocation);
                    rotationHelper.OrientationChanged += OnOrientationChanged;
                    await StartPreviewAsync();
                }
            }
        }

        private async Task CleanupCameraAsync()
        {
            if (isInitialized)
            {
                if (isPreviewing)
                {
                    await StopPreviewAsync();
                }

                if (mediaFrameReader != null)
                {
                    mediaFrameReader.FrameArrived -= MediaFrameReader_FrameArrived;
                    await mediaFrameReader.StopAsync();
                }

                isInitialized = false;
            }

            if (mediaCapture != null)
            {
                mediaCapture.Dispose();
                mediaCapture = null;
            }

            if (rotationHelper != null)
            {
                rotationHelper.OrientationChanged -= OnOrientationChanged;
                rotationHelper = null;
            }
        }

        private async void MediaFrameReader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            if (processing)
            {
                return;
            }

            using (var mediaFrameReference = sender.TryAcquireLatestFrame())
            {
                if (mediaFrameReference?.VideoMediaFrame != null)
                {
                    using (var videoFrame = mediaFrameReference.VideoMediaFrame.GetVideoFrame())
                    {
                        if (videoFrame.Direct3DSurface != null)
                        {
                            processing = true;
                            await cameraPreviewControl.OnMediaFrameArrived(new MediaFrame(videoFrame.Direct3DSurface));
                            processing = false;
                        }
                    }
                }
            }
        }

        private async Task StartPreviewAsync()
        {
            // Prevent the device from sleeping while the preview is running
            displayRequest.RequestActive();

            // Set the preview source in the UI and mirror if necessary
            captureElement.Source = mediaCapture;
            captureElement.FlowDirection = mirroringPreview ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            // Start preview
            await mediaCapture.StartPreviewAsync();
            isPreviewing = true;

            // Initialize preview to current orientation
            await SetPreviewRotationAsync();
        }

        private async Task SetPreviewRotationAsync()
        {
            // Only update the orientation if the camera is mounted on the device
            if (externalCamera)
            {
                return;
            }

            // Add rotation metadata to the preview stream to ensure aspect ratio/dimensions match when rendering and getting preview frames
            var orientation = rotationHelper.GetCameraPreviewOrientation();
            var properties = mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
            properties.Properties.Add(RotationKey, CameraRotationHelper.ConvertSimpleOrientationToClockwiseDegrees(orientation));
            await mediaCapture.SetEncodingPropertiesAsync(MediaStreamType.VideoPreview, properties, null);
        }

        private async Task StopPreviewAsync()
        {
            isPreviewing = false;
            await mediaCapture.StopPreviewAsync();

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                captureElement.Source = null;

                // Allow the device screen to sleep now that the preview is stopped
                displayRequest.RequestRelease();
            });
        }

        private async void OnOrientationChanged(object sender, bool updatePreview)
        {
            if (updatePreview)
            {
                await SetPreviewRotationAsync();
            }

            // await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => UpdateButtonOrientation());
        }

        private async void OnAppResuming(object sender, object e)
        {
            isSuspending = false;
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {
                await SetupBasedOnStateAsync();
            });
        }

        private async void OnAppSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            isSuspending = true;
            var deferral = e.SuspendingOperation.GetDeferral();
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {
                await SetupBasedOnStateAsync();
                deferral.Complete();
            });
        }

        private async Task EnableContinuousAutoFocusAsync(bool enable)
        {
            var focusControl = mediaCapture.VideoDeviceController.FocusControl;
            if (focusControl.SupportedFocusModes.Contains(FocusMode.Continuous))
            {
                await focusControl.UnlockAsync();
                var settings = new FocusSettings { Mode = enable ? FocusMode.Continuous : FocusMode.Auto, AutoFocusRange = AutoFocusRange.FullRange };
                focusControl.Configure(settings);
                await focusControl.FocusAsync();
            }
        }

        private IReadOnlyList<CameraFocusMode> RetrieveSupportedFocusModes()
        {
            var supportedFocusModes = new List<CameraFocusMode>();
            foreach (var supportedFocusMode in mediaCapture.VideoDeviceController.FocusControl.SupportedFocusModes)
            {
                switch (supportedFocusMode)
                {
                    case FocusMode.Auto:
                        supportedFocusModes.Add(CameraFocusMode.Auto);
                        break;

                    case FocusMode.Single:

                        // Not supported in other platforms so leave it out
                        break;

                    case FocusMode.Continuous:
                        supportedFocusModes.Add(CameraFocusMode.Continuous);
                        break;

                    case FocusMode.Manual:
                        supportedFocusModes.Add(CameraFocusMode.Manual);
                        break;
                }
            }

            return supportedFocusModes.AsReadOnly();
        }

        private async Task<IReadOnlyList<ICamera>> RetrieveCamerasAsync()
        {
            var cameras = new List<Camera>();
            var videoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            if (videoDevices != null)
            {
                foreach (var videoDevice in videoDevices)
                {
                    var camera = new Camera()
                    {
                        Id = videoDevice.Id,
                        CameraFacing = ToCameraFacing(videoDevice.EnclosureLocation?.Panel ?? Panel.Unknown)
                    };
                    cameras.Add(camera);
                }
            }

            return cameras.AsReadOnly();
        }
    }
}