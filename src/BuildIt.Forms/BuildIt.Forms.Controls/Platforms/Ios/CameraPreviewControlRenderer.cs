using AVFoundation;
using BuildIt.Forms.Controls;
using BuildIt.Forms.Controls.Platforms.Ios;
using Foundation;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CameraPreviewControl), typeof(CameraPreviewControlRenderer))]
namespace BuildIt.Forms.Controls.Platforms.Ios
{
    /// <inheritdoc />
    public class CameraPreviewControlRenderer : FrameRenderer
    {
        private AVCaptureSession captureSession;
        private AVCaptureDeviceInput captureDeviceInput;
        private AVCaptureDevice captureDevice;
        private AVCaptureStillImageOutput stillImageOutput;
        private UIView liveCameraStream;

        private bool isInitialized;

        private CameraPreviewControl cameraPreviewControl;
        private AVCaptureFocusMode defaultFocusMode;

        /// <inheritdoc />
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            // If our bounds have changed, make sure we update all of our subviews as well
            // (ie. when the screen rotates, or if the size of the control has changed) -RR
            if (liveCameraStream == null)
            {
                return;
            }

            liveCameraStream.Frame = Bounds;

            if (liveCameraStream.Layer?.Sublayers == null)
            {
                return;
            }

            foreach (var layerSublayer in liveCameraStream.Layer.Sublayers)
            {
                layerSublayer.Frame = liveCameraStream.Bounds;
            }
        }

        /// <inheritdoc />
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            var cpc = Element as CameraPreviewControl;
            if (cpc == null)
            {
                return;
            }

            if (cameraPreviewControl != null)
            {
                cameraPreviewControl.CaptureNativeFrameToFileDelegate = null;
            }

            cameraPreviewControl = cpc;
            cameraPreviewControl.CaptureNativeFrameToFileDelegate = CapturePhotoToFile;

            SetupUserInterface();
            SetupEventHandlers();

            if (!Element.IsVisible)
            {
                return;
            }

            try
            {
                SetupLiveCameraStream();
                AuthorizeCameraUse();
                isInitialized = true;
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
        }

        /// <inheritdoc />
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName)
            {
                try
                {
                    if (!isInitialized &&
                        Element.IsVisible)
                    {
                        SetupLiveCameraStream();
                        AuthorizeCameraUse();
                        isInitialized = true;
                    }
                    else if (isInitialized && !Element.IsVisible)
                    {
                        StopCameraFeed();
                    }
                }
                catch (Exception ex)
                {
                    ex.LogError();
                }
            }

            if (e.PropertyName == CameraPreviewControl.PreferredCameraProperty.PropertyName &&
                isInitialized)
            {
                SetPreferredCamera();
            }

            if (e.PropertyName == CameraPreviewControl.EnableContinuousAutoFocusProperty.PropertyName)
            {
                EnableContinuousAutofocus(cameraPreviewControl.EnableContinuousAutoFocus);
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            StopCameraFeed();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Captures the most current video frame to a photo and saves it to local storage
        /// </summary>
        /// <param name="saveToPhotosLibrary">Whether or not to add the file to the device's photo library.
        /// **If Saving to Photos Library** Requires `NSPhotoLibraryUsageDescription' in info.plist</param>
        /// <returns>The path to the saved photo file</returns>
        protected virtual async Task<string> CapturePhotoToFile(bool saveToPhotosLibrary)
        {
            var videoConnection = stillImageOutput.ConnectionFromMediaType(AVMediaType.Video);
            var sampleBuffer = await stillImageOutput.CaptureStillImageTaskAsync(videoConnection);
            var jpegImage = AVCaptureStillImageOutput.JpegStillToNSData(sampleBuffer);

            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "VideoCapture", DateTime.Now.ToString("yyyy-MM-dd"));
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var fileCount = Directory.GetFiles(folder).Length;
            var fileName = Path.Combine(folder, $"{fileCount}.jpg");
            File.Create(fileName).Dispose();

            jpegImage.Save(fileName, false, out var error);

            if (error != null)
            {
                return error.ToString();
            }

            if (saveToPhotosLibrary)
            {
                var photo = new UIImage(jpegImage);
                photo.SaveToPhotosAlbum((img, err) => error = err);
            }

            return error == null ? fileName : error.ToString();
        }

        private void SetPreferredCamera()
        {
            captureDevice = GetCameraForPreference(cameraPreviewControl.PreferredCamera);
            System.Diagnostics.Debug.WriteLine($"focus mode {captureDevice.FocusMode}");
            ConfigureCameraForDevice();

            captureSession.BeginConfiguration();
            captureSession.RemoveInput(captureDeviceInput);
            captureDeviceInput = AVCaptureDeviceInput.FromDevice(captureDevice);
            captureSession.AddInput(captureDeviceInput);
            captureSession.CommitConfiguration();
        }

        private void StopCameraFeed()
        {
            if (captureDeviceInput != null)
            {
                captureSession?.RemoveInput(captureDeviceInput);
                captureDeviceInput.Dispose();
                captureDeviceInput = null;
            }

            if (captureSession != null)
            {
                captureSession.StopRunning();
                captureSession.Dispose();
                captureSession = null;
            }

            if (stillImageOutput != null)
            {
                stillImageOutput.Dispose();
                stillImageOutput = null;
            }

            isInitialized = false;
        }

        private void SetupUserInterface()
        {
            liveCameraStream = new UIView { Frame = Frame };
            Add(liveCameraStream);
        }

        private void SetupEventHandlers()
        {
        }

        private void SetupLiveCameraStream()
        {
            captureSession = new AVCaptureSession();

            var videoPreviewLayer = new AVCaptureVideoPreviewLayer(captureSession)
            {
                Frame = liveCameraStream.Bounds
            };

            liveCameraStream.Layer.AddSublayer(videoPreviewLayer);

            captureDevice = GetCameraForPreference(cameraPreviewControl.PreferredCamera);
            
            ConfigureCameraForDevice();
            captureDeviceInput = AVCaptureDeviceInput.FromDevice(captureDevice);

            stillImageOutput = new AVCaptureStillImageOutput
            {
                OutputSettings = new NSDictionary(AVVideo.CodecKey, AVVideo.CodecJPEG)
            };

            captureSession.AddOutput(stillImageOutput);
            captureSession.AddInput(captureDeviceInput);
            captureSession.StartRunning();
        }

        private AVCaptureDevice GetCameraForPreference(CameraPreviewControl.CameraPreference cameraPreference)
        {
            var orientation = cameraPreference == CameraPreviewControl.CameraPreference.Back
                ? AVCaptureDevicePosition.Back
                : AVCaptureDevicePosition.Front;

            var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
            return devices.FirstOrDefault(d => d.Position == orientation) ?? devices.FirstOrDefault(d => d.Position == AVCaptureDevicePosition.Unspecified);
        }

        private void ConfigureCameraForDevice()
        {
            var error = new NSError();
            if (captureDevice.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
            {
                captureDevice.LockForConfiguration(out error);
                captureDevice.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
                captureDevice.UnlockForConfiguration();
            }
            else if (captureDevice.IsExposureModeSupported(AVCaptureExposureMode.ContinuousAutoExposure))
            {
                captureDevice.LockForConfiguration(out error);
                captureDevice.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
                captureDevice.UnlockForConfiguration();
            }
            else if (captureDevice.IsWhiteBalanceModeSupported(AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance))
            {
                captureDevice.LockForConfiguration(out error);
                captureDevice.WhiteBalanceMode = AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance;
                captureDevice.UnlockForConfiguration();
            }
        }

        private async void AuthorizeCameraUse()
        {
            var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);
            if (authorizationStatus != AVAuthorizationStatus.Authorized)
            {
                await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
            }
        }

        private void EnableContinuousAutofocus(bool enable)
        {
            if (captureDevice.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
            {
                captureDevice.LockForConfiguration(out NSError error);
                captureDevice.FocusMode = enable ? AVCaptureFocusMode.ContinuousAutoFocus : AVCaptureFocusMode.AutoFocus;
                captureDevice.UnlockForConfiguration();
            }
        }
    }
}