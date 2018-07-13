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
        private AVCaptureStillImageOutput stillImageOutput;
        private UIView liveCameraStream;

        private bool isInitialized;

        private CameraPreviewControl cameraPreviewControl;

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
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            StopCameraFeed();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Captures the current video frame to a photo file
        /// </summary>
        /// <returns>Path to the captured storage file</returns>
        protected virtual async Task<string> CapturePhotoToFile()
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

            var photo = new UIImage(jpegImage);
            photo.SaveToPhotosAlbum((img, err) =>
            {
                error = err;
            });

            return error == null ? fileName : error.ToString();
        }

        private void SetPreferredCamera()
        {
            var device = GetCameraForPreference(cameraPreviewControl.PreferredCamera);
            ConfigureCameraForDevice(device);

            captureSession.BeginConfiguration();
            captureSession.RemoveInput(captureDeviceInput);
            captureDeviceInput = AVCaptureDeviceInput.FromDevice(device);
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

            var captureDevice = GetCameraForPreference(cameraPreviewControl.PreferredCamera);
            ConfigureCameraForDevice(captureDevice);
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

        private void ConfigureCameraForDevice(AVCaptureDevice device)
        {
            var error = new NSError();
            if (device.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
            {
                device.LockForConfiguration(out error);
                device.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
                device.UnlockForConfiguration();
            }
            else if (device.IsExposureModeSupported(AVCaptureExposureMode.ContinuousAutoExposure))
            {
                device.LockForConfiguration(out error);
                device.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
                device.UnlockForConfiguration();
            }
            else if (device.IsWhiteBalanceModeSupported(AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance))
            {
                device.LockForConfiguration(out error);
                device.WhiteBalanceMode = AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance;
                device.UnlockForConfiguration();
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
    }
}