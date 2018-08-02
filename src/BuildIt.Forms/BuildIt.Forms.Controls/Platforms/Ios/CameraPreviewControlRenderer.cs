using AVFoundation;
using BuildIt.Forms.Controls;
using BuildIt.Forms.Controls.Platforms.Ios;
using CoreFoundation;
using CoreMedia;
using CoreVideo;
using Foundation;
using System;
using System.Collections.Generic;
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
    public class CameraPreviewControlRenderer : FrameRenderer/*, IAVCaptureVideoDataOutputSampleBufferDelegate*/
    {
        private AVCaptureSession captureSession;
        private AVCaptureDeviceInput captureDeviceInput;
        private AVCaptureDevice captureDevice;
        private AVCaptureStillImageOutput stillImageOutput;
        private UIView liveCameraStream;
        private bool isInitialized;
        private CameraPreviewControl cameraPreviewControl;
        AVCaptureVideoDataOutput videoOutput;
        FrameExtractor frameExtractor;
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

            cameraPreviewControl = cpc;
            cameraPreviewControl.CaptureNativeFrameToFileFunc = CapturePhotoToFile;
            cameraPreviewControl.RetrieveSupportedFocusModesFunc = RetrieveSupportedFocusModes;
            cameraPreviewControl.RetrieveCamerasFunc = RetrieveCamerasAsync;
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

        private static CameraPreviewControl.CameraFacing ToCameraFacing(AVCaptureDevicePosition position)
        {
            switch (position)
            {
                case AVCaptureDevicePosition.Back:
                    return CameraPreviewControl.CameraFacing.Back;

                case AVCaptureDevicePosition.Front:
                    return CameraPreviewControl.CameraFacing.Front;

                default:
                    return CameraPreviewControl.CameraFacing.Unspecified;
            }
        }

        private void SetPreferredCamera()
        {
            captureDevice = GetCameraForPreference(cameraPreviewControl.PreferredCamera);
            ConfigureCameraForDevice();

            captureSession.BeginConfiguration();
            captureSession.RemoveInput(captureDeviceInput);
            captureDeviceInput = AVCaptureDeviceInput.FromDevice(captureDevice);
            captureSession.AddInput(captureDeviceInput);
            captureSession.CommitConfiguration();
        }

        private void StopCameraFeed()
        {
            captureSession?.RemoveInput(captureDeviceInput);
            captureDeviceInput?.Dispose();
            captureDeviceInput = null;

            captureSession?.StopRunning();
            captureSession?.Dispose();
            captureSession = null;

            stillImageOutput?.Dispose();
            stillImageOutput = null;

            videoOutput?.Dispose();
            videoOutput = null;
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
            videoOutput = new AVCaptureVideoDataOutput()
            {
                AlwaysDiscardsLateVideoFrames = true
            };
            var videoCaptureQueue = new DispatchQueue("Video Capture Queue");
            videoOutput.SetSampleBufferDelegateQueue(new FrameExtractor(cameraPreviewControl.OnMediaFrameArrived), videoCaptureQueue);
            if (captureSession.CanAddOutput(videoOutput))
            {
                captureSession.AddOutput(videoOutput);
            }

            captureSession.AddOutput(stillImageOutput);
            captureSession.AddInput(captureDeviceInput);
            captureSession.StartRunning();
        }

        private AVCaptureDevice GetCameraForPreference(CameraPreviewControl.CameraFacing cameraPreference)
        {
            var orientation = cameraPreference == CameraPreviewControl.CameraFacing.Back
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

        private IReadOnlyList<CameraFocusMode> RetrieveSupportedFocusModes()
        {
            var supportedFocusModes = new List<CameraFocusMode>();
            if (captureDevice.IsFocusModeSupported(AVCaptureFocusMode.AutoFocus))
            {
                supportedFocusModes.Add(CameraFocusMode.Auto);
            }

            if (captureDevice.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
            {
                supportedFocusModes.Add(CameraFocusMode.Continuous);
            }

            if (captureDevice.FocusPointOfInterestSupported)
            {
                supportedFocusModes.Add(CameraFocusMode.Manual);
            }

            return supportedFocusModes.AsReadOnly();
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<IReadOnlyList<ICamera>> RetrieveCamerasAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var cameras = new List<ICamera>();
            var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
            if (devices != null)
            {
                foreach (var device in devices)
                {
                    var camera = new Camera()
                    {
                        Id = device.UniqueID,
                        CameraFacing = ToCameraFacing(device.Position)
                    };
                    cameras.Add(camera);
                }
            }

            return cameras;
        }

        //[Export("captureOutput:didOutputSampleBuffer:fromConnection:")]
        //private void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
        //{
        //    System.Diagnostics.Debug.WriteLine("output sample buffer");
        //    var pixelBuffer = sampleBuffer.GetImageBuffer() as CVPixelBuffer;
        //    if (pixelBuffer != null)
        //    {
        //        cameraPreviewControl.OnMediaFrameArrived(new MediaFrame(pixelBuffer));
        //    }
        //}
    }

    internal class FrameExtractor : AVCaptureVideoDataOutputSampleBufferDelegate
    {
        private readonly Action<MediaFrame> frameArrivedAction;

        public FrameExtractor(Action<MediaFrame> frameArrivedAction)
        {
            this.frameArrivedAction = frameArrivedAction;
        }

        public override void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
        {
            var pixelBuffer = sampleBuffer.GetImageBuffer() as CVPixelBuffer;
            if (pixelBuffer != null)
            {
                frameArrivedAction(new MediaFrame(pixelBuffer));
            }
        }
    }
}