using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AVFoundation;
using BuildIt.Forms.Controls;
using BuildIt.Forms.Controls.Common;
using BuildIt.Forms.Controls.Extensions;
using BuildIt.Forms.Controls.Interfaces;
using BuildIt.Forms.Controls.Platforms.Ios;
using BuildIt.Forms.Controls.Platforms.Ios.Extensions;
using BuildIt.Forms.Controls.Platforms.Ios.Models;
using BuildIt.Forms.Parameters;
using CoreFoundation;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CameraPreviewControl), typeof(CameraPreviewControlRenderer))]
namespace BuildIt.Forms.Controls.Platforms.Ios
{
    /// <inheritdoc />
    public class CameraPreviewControlRenderer : FrameRenderer, IAVCaptureVideoDataOutputSampleBufferDelegate
    {
        private const string ImageCapture = "ImageCapture";
        private AVCaptureSession captureSession;
        private AVCaptureVideoPreviewLayer videoPreviewLayer;
        private AVCaptureDeviceInput captureDeviceInput;
        private AVCaptureDevice captureDevice;
        private AVCaptureStillImageOutput stillImageOutput;
        private UIView liveCameraStream;
        private bool isInitialized;
        private CameraPreviewControl cameraPreviewControl;
        private AVCaptureVideoDataOutput videoOutput;
        private FrameExtractor frameExtractor;
        private DispatchQueue videoCaptureQueue;

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
            cameraPreviewControl.StartPreviewFunc = StartPreviewFunc;
            cameraPreviewControl.StopPreviewFunc = StopPreviewFunc;
            cameraPreviewControl.SetFocusModeFunc = SetFocusModeFunc;
            cameraPreviewControl.TryFocusingFunc = TryFocusingFunc;
            cameraPreviewControl.CaptureNativeFrameToFileFunc = CapturePhotoToFile;
            cameraPreviewControl.RetrieveSupportedFocusModesFunc = RetrieveSupportedFocusModes;
            cameraPreviewControl.RetrieveCamerasFunc = RetrieveCamerasAsync;

            SetupUserInterface();
        }

        /// <inheritdoc />
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == CameraPreviewControl.PreferredCameraProperty.PropertyName &&
                isInitialized)
            {
                SetPreferredCamera();
            }

            if (e.PropertyName == CameraPreviewControl.AspectProperty.PropertyName)
            {
                ApplyAspect();
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            StopPreviewFunc().RunSynchronously();
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
            NSError error = null;
            try
            {
                using (var videoConnection = stillImageOutput.ConnectionFromMediaType(AVMediaType.Video))
                {
                    using (var sampleBuffer = await stillImageOutput.CaptureStillImageTaskAsync(videoConnection))
                    {
                        using (var jpegImage = AVCaptureStillImageOutput.JpegStillToNSData(sampleBuffer))
                        {
                            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ImageCapture, DateTime.Now.ToString("yyyy-MM-dd"));
                            if (!Directory.Exists(folder))
                            {
                                Directory.CreateDirectory(folder);
                            }

                            var fileCount = Directory.GetFiles(folder).Length;
                            var fileName = Path.Combine(folder, $"{fileCount}.jpg");
                            using (File.Create(fileName))
                            {
                                jpegImage.Save(fileName, false, out error);

                                if (error != null)
                                {
                                    return error.ToString();
                                }

                                if (saveToPhotosLibrary)
                                {
                                    using (var photo = new UIImage(jpegImage))
                                    {
                                        photo.SaveToPhotosAlbum((img, err) => error = err);
                                    }
                                }

                                return error == null ? fileName : error.ToString();
                            }
                        }
                    }
                }
            }
            finally
            {
                error?.Dispose();
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task StartPreviewFunc()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            SetupLiveCameraStream();
            isInitialized = true;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task StopPreviewFunc(ICameraPreviewStopParameters parameters = null)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            StopCameraFeed();
        }

#pragma warning disable 1998 // Async method lacks 'await' operators and will run synchronously
        private async Task SetFocusModeFunc()
#pragma warning restore 1998 // Async method lacks 'await' operators and will run synchronously
        {
            SetFocusModeAsync(cameraPreviewControl.FocusMode);
        }

        private void SetFocusModeAsync(CameraFocusMode controlFocusMode)
        {
            var focusMode = controlFocusMode.ToPlatformFocusMode();

            if (controlFocusMode == CameraFocusMode.Unspecified)
            {
                focusMode = RetrieveSupportedFocusModes().LastOrDefault().ToPlatformFocusMode();
            }

            if (captureDevice.IsFocusModeSupported(focusMode))
            {
                captureDevice.LockForConfiguration(out NSError error);
                captureDevice.FocusMode = focusMode;
                captureDevice.UnlockForConfiguration();
            }
            else
            {
                focusMode = RetrieveSupportedFocusModes().LastOrDefault().ToPlatformFocusMode();
                var fallbackFocusMode = focusMode.ToControlFocusMode();
                cameraPreviewControl.ErrorCommand?.Execute(new CameraPreviewControlErrorParameters<CameraFocusMode>(new[] { string.Format(Common.Constants.Errors.UnsupportedFocusModeFormat, controlFocusMode, fallbackFocusMode) }, fallbackFocusMode, true));
            }
        }

#pragma warning disable 1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<bool> TryFocusingFunc()
#pragma warning restore 1998 // Async method lacks 'await' operators and will run synchronously
        {
            try
            {
                if (captureDevice.FocusMode == AVCaptureFocusMode.Locked)
                {
                    captureDevice.LockForConfiguration(out NSError error);
                    captureDevice.FocusMode = AVCaptureFocusMode.AutoFocus;
                    captureDevice.UnlockForConfiguration();
                }

                return true;
            }
            catch (Exception ex)
            {
                cameraPreviewControl.ErrorCommand?.Execute(new CameraPreviewControlErrorParameters(new[] { Common.Constants.Errors.CameraFocusingFailed, ex.Message }));
            }

            return false;
        }

        private static CameraFacing ToCameraFacing(AVCaptureDevicePosition position)
        {
            switch (position)
            {
                case AVCaptureDevicePosition.Back:
                    return CameraFacing.Back;

                case AVCaptureDevicePosition.Front:
                    return CameraFacing.Front;

                default:
                    return CameraFacing.Unspecified;
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

            videoCaptureQueue?.Dispose();
            videoCaptureQueue = null;
            isInitialized = false;

            cameraPreviewControl.SetStatus(CameraStatus.Stopped);
        }

        private void SetupUserInterface()
        {
            liveCameraStream = new UIView { Frame = Frame };
            Add(liveCameraStream);
        }

        private void SetupLiveCameraStream()
        {
            captureSession = new AVCaptureSession();

            videoPreviewLayer = new AVCaptureVideoPreviewLayer(captureSession)
            {
                Frame = liveCameraStream.Bounds,
            };
            ApplyAspect();
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
            videoCaptureQueue = new DispatchQueue("Video Capture Queue");
            frameExtractor = new FrameExtractor(cameraPreviewControl.OnMediaFrameArrived);
            videoOutput.SetSampleBufferDelegateQueue(frameExtractor, videoCaptureQueue);
            captureSession.AddInput(captureDeviceInput);

            if (captureSession.CanAddOutput(videoOutput))
            {
                captureSession.AddOutput(videoOutput);
            }

            captureSession.AddOutput(stillImageOutput);
            captureSession.StartRunning();

            cameraPreviewControl.SetStatus(CameraStatus.Started);
        }

        private void ApplyAspect()
        {
            if (cameraPreviewControl == null)
            {
                return;
            }

            switch (cameraPreviewControl.Aspect)
            {
                case Aspect.AspectFill:
                    videoPreviewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
                    break;
                case Aspect.AspectFit:
                    videoPreviewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspect;
                    break;
                case Aspect.Fill:
                    videoPreviewLayer.VideoGravity = AVLayerVideoGravity.Resize;
                    break;
            }
        }

        private AVCaptureDevice GetCameraForPreference(CameraFacing cameraPreference)
        {
            var orientation = cameraPreference == CameraFacing.Back
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
    }
}