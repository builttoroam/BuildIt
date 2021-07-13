using AVFoundation;
using CoreFoundation;
using Foundation;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace BuildIt.Media.Uno.WinUI
{
    /// <inheritdoc />
    public partial class CameraPreview : IAVCaptureVideoDataOutputSampleBufferDelegate
    {
        private readonly SemaphoreSlim tryFocusingSemaphoreSlim = new SemaphoreSlim(1);
        private readonly IntPtr focusModeChangeObserverContext = (IntPtr)1;

        private const string ImageCapture = "ImageCapture";
        private const string FocusModeObserverValueName = "focusMode";

        private AVCaptureSession captureSession;
        private AVCaptureVideoPreviewLayer videoPreviewLayer;
        private AVCaptureDeviceInput captureDeviceInput;
        private AVCaptureDevice captureDevice;
        private AVCaptureStillImageOutput stillImageOutput;
        //private UIView nativePreviewElement;
        private bool isInitialized;
        private AVCaptureVideoDataOutput videoOutput;
        private FrameExtractor frameExtractor;
        private DispatchQueue videoCaptureQueue;

        private TaskCompletionSource<bool> autoFocusedCompletionSourceTask;

        partial void PlatformInitCameraPreview()
        {
            var cameraPreviewControl = this;
            cameraPreviewControl.StartPreviewFunc = PlatformStartPreviewFunc;
            cameraPreviewControl.StopPreviewFunc = PlatformStopPreviewFunc;
            cameraPreviewControl.SetFocusModeFunc = PlatformSetFocusModeFunc;
            cameraPreviewControl.TryFocusingFunc = PlatformTryFocusingFunc;
            cameraPreviewControl.CaptureNativeFrameToFileFunc = PlatformCapturePhotoToFile;
            cameraPreviewControl.RetrieveSupportedFocusModesFunc = PlatformRetrieveSupportedFocusModes;
            cameraPreviewControl.RetrieveCamerasFunc = PlatformRetrieveCamerasAsync;

            //mTextureView = new AutoFitTextureView(ContextHelper.Current);
            //var wrapped = VisualTreeHelper.AdaptNative(mTextureView);
            //RootBorder.Child = wrapped;

            //SetupUserInterface();
        }

        //private Task<IReadOnlyList<ICamera>> PlatformRetrieveCamerasAsync()
        //{
        //    throw new NotImplementedException();
        //}

        //private IReadOnlyList<CameraFocusMode> PlatformRetrieveSupportedFocusModes()
        //{
        //    throw new NotImplementedException();
        //}

        //private Task<string> PlatformCapturePhotoToFile(bool arg)
        //{
        //    throw new NotImplementedException();
        //}

        //private Task<bool> PlatformTryFocusingFunc()
        //{
        //    throw new NotImplementedException();
        //}

        //private Task PlatformSetFocusModeFunc()
        //{
        //    throw new NotImplementedException();
        //}

        //private async Task PlatformStopPreviewFunc(ICameraPreviewStopParameters arg)
        //{
        //    //throw new NotImplementedException();
        //}

        //private async Task PlatformStartPreviewFunc()
        //{
        //}


        /// <inheritdoc />
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            // If our bounds have changed, make sure we update all of our subviews as well
            // (ie. when the screen rotates, or if the size of the control has changed) -RR
            if (nativePreviewElement == null)
            {
                return;
            }

            nativePreviewElement.Frame = Bounds;

            if (nativePreviewElement.Layer?.Sublayers == null)
            {
                return;
            }

            foreach (var layerSublayer in nativePreviewElement.Layer.Sublayers)
            {
                layerSublayer.Frame = nativePreviewElement.Bounds;
            }
        }

        /// <inheritdoc />
        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            if (context == focusModeChangeObserverContext && string.Equals(keyPath, FocusModeObserverValueName))
            {
                var oldValue = -1;
                var newValue = -1;
                foreach (var key in change.Keys)
                {
                    var value = change[key];
                    if (value is NSNumber number)
                    {
                        Debug.WriteLine($"[{nameof(ObserveValue)}] Value for key {key as NSString} is {((AVCaptureFocusMode)number.Int32Value).ToString()}");
                        if (string.Equals(key as NSString, nameof(NSKeyValueObservingOptions.New).ToLower()))
                        {
                            newValue = number.Int32Value;
                        }
                        else if (string.Equals(key as NSString, nameof(NSKeyValueObservingOptions.Old).ToLower()))
                        {
                            oldValue = number.Int32Value;
                        }
                    }
                }

                if (oldValue == (int)AVCaptureFocusMode.AutoFocus)
                {
                    autoFocusedCompletionSourceTask?.SetResult(newValue == (int)AVCaptureFocusMode.Locked);
                }
            }
            else
            {
                base.ObserveValue(keyPath, ofObject, change, context);
            }
        }

        ///// <inheritdoc />
        //protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        //{
        //    base.OnElementChanged(e);

        //    var cpc = Element as CameraPreviewControl;
        //    if (cpc == null)
        //    {
        //        return;
        //    }

        //    cameraPreviewControl = cpc;
        //    cameraPreviewControl.StartPreviewFunc = StartPreviewFunc;
        //    cameraPreviewControl.StopPreviewFunc = StopPreviewFunc;
        //    cameraPreviewControl.SetFocusModeFunc = SetFocusModeFunc;
        //    cameraPreviewControl.TryFocusingFunc = TryFocusingFunc;
        //    cameraPreviewControl.CaptureNativeFrameToFileFunc = CapturePhotoToFile;
        //    cameraPreviewControl.RetrieveSupportedFocusModesFunc = RetrieveSupportedFocusModes;
        //    cameraPreviewControl.RetrieveCamerasFunc = RetrieveCamerasAsync;

        //    SetupUserInterface();
        //}

        /// <inheritdoc />
        //protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    base.OnElementPropertyChanged(sender, e);

        //    if (e.PropertyName == CameraPreviewControl.PreferredCameraProperty.PropertyName &&
        //        isInitialized)
        //    {
        //        await SetPreferredCamera();
        //    }

        //    if (e.PropertyName == CameraPreviewControl.AspectProperty.PropertyName)
        //    {
        //        ApplyAspect();
        //    }
        //}

        ///// <inheritdoc />
        //protected override void Dispose(bool disposing)
        //{
        //    StopPreviewFunc().RunSynchronously();
        //    base.Dispose(disposing);
        //}

        /// <summary>
        /// Captures the most current video frame to a photo and saves it to local storage.
        /// </summary>
        /// <param name="saveToPhotosLibrary">Whether or not to add the file to the device's photo library.
        /// **If Saving to Photos Library** Requires `NSPhotoLibraryUsageDescription' in info.plist.</param>
        /// <returns>The path to the saved photo file.</returns>
        protected virtual async Task<string> PlatformCapturePhotoToFile(bool saveToPhotosLibrary)
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

        private async Task PlatformStartPreviewFunc()
        {
            await SetupLiveCameraStream();
            isInitialized = true;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task PlatformStopPreviewFunc(ICameraPreviewStopParameters parameters = null)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            StopCameraFeed();
        }

#pragma warning disable 1998 // Async method lacks 'await' operators and will run synchronously
        private async Task PlatformSetFocusModeFunc()
#pragma warning restore 1998 // Async method lacks 'await' operators and will run synchronously
        {
            SetFocusModeAsync(FocusMode);
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
                ErrorCommand?.Execute(new CameraPreviewControlErrorParameters<CameraFocusMode>(new[] { string.Format(Constants.Errors.UnsupportedFocusModeFormat, controlFocusMode, fallbackFocusMode) }, fallbackFocusMode, true));
            }
        }

        private async Task<bool> PlatformTryFocusingFunc()
        {
            if (captureDevice.FocusMode == AVCaptureFocusMode.ContinuousAutoFocus)
            {
                return false;
            }

            try
            {
                await tryFocusingSemaphoreSlim.WaitAsync();

                autoFocusedCompletionSourceTask = new TaskCompletionSource<bool>();

                if (captureDevice.FocusMode == AVCaptureFocusMode.Locked)
                {
                    captureDevice.LockForConfiguration(out NSError error);
                    captureDevice.FocusMode = AVCaptureFocusMode.AutoFocus;
                    captureDevice.UnlockForConfiguration();
                }

                return await autoFocusedCompletionSourceTask.Task;
            }
            catch (Exception ex)
            {
                ErrorCommand?.Execute(new CameraPreviewControlErrorParameters(new[] { Constants.Errors.CameraFocusingFailed, ex.Message }));
            }
            finally
            {
                autoFocusedCompletionSourceTask = null;
                tryFocusingSemaphoreSlim.Release();
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

        private async Task SetPreferredCamera()
        {
            captureDevice = GetCameraForPreference(PreferredCamera);
            await ConfigureCameraForDevice();

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

            this.SetStatus(CameraStatus.Stopped);
        }

        //private void SetupUserInterface()
        //{
        //    //nativePreviewElement = new UIView { Frame = Frame };

        //    //var wrapped = VisualTreeHelper.AdaptNative(nativePreviewElement);
        //    //RootBorder.Child = wrapped;
        //}

        private async Task SetupLiveCameraStream()
        {
            captureSession = new AVCaptureSession();

            videoPreviewLayer = new AVCaptureVideoPreviewLayer(captureSession)
            {
                Frame = nativePreviewElement.Bounds,
            };
            ApplyAspect();
            nativePreviewElement.Layer.AddSublayer(videoPreviewLayer);

            captureDevice = GetCameraForPreference(this.PreferredCamera);

            await ConfigureCameraForDevice();
            captureDeviceInput = AVCaptureDeviceInput.FromDevice(captureDevice);

            stillImageOutput = new AVCaptureStillImageOutput
            {
                OutputSettings = new NSDictionary(AVVideo.CodecKey, AVVideo.CodecJPEG),
            };
            videoOutput = new AVCaptureVideoDataOutput()
            {
                AlwaysDiscardsLateVideoFrames = true,
            };
            videoCaptureQueue = new DispatchQueue("Video Capture Queue");
            frameExtractor = new FrameExtractor(OnMediaFrameArrived);
            videoOutput.SetSampleBufferDelegateQueue(frameExtractor, videoCaptureQueue);
            captureSession.AddInput(captureDeviceInput);

            if (captureSession.CanAddOutput(videoOutput))
            {
                captureSession.AddOutput(videoOutput);
            }

            captureSession.AddOutput(stillImageOutput);
            captureSession.StartRunning();

            this.SetStatus(CameraStatus.Started);
        }

        private void ApplyAspect()
        {
            switch (Stretch)
            {
                case Microsoft.UI.Xaml.Media.Stretch.UniformToFill:
                    videoPreviewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
                    break;

                case Microsoft.UI.Xaml.Media.Stretch.Uniform:
                    videoPreviewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspect;
                    break;

                case Microsoft.UI.Xaml.Media.Stretch.Fill:
                    videoPreviewLayer.VideoGravity = AVLayerVideoGravity.Resize;
                    break;
            }
        }

        private AVCaptureDevice GetCameraForPreference(CameraFacing cameraPreference)
        {
            captureDevice?.RemoveObserver(this, FocusModeObserverValueName);

            var orientation = cameraPreference == CameraFacing.Back
                ? AVCaptureDevicePosition.Back
                : AVCaptureDevicePosition.Front;

            var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
            var device = devices.FirstOrDefault(d => d.Position == orientation) ?? devices.FirstOrDefault(d => d.Position == AVCaptureDevicePosition.Unspecified);
            device?.AddObserver(this, FocusModeObserverValueName, NSKeyValueObservingOptions.OldNew, focusModeChangeObserverContext);

            return device;
        }

        private async Task ConfigureCameraForDevice()
        {
            var error = new NSError();
            await SetFocusModeFunc();
            if (captureDevice.IsExposureModeSupported(AVCaptureExposureMode.ContinuousAutoExposure))
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

        private IReadOnlyList<CameraFocusMode> PlatformRetrieveSupportedFocusModes()
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
        private async Task<IReadOnlyList<ICamera>> PlatformRetrieveCamerasAsync()
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
                        CameraFacing = ToCameraFacing(device.Position),
                    };
                    cameras.Add(camera);
                }
            }

            return cameras;
        }
    }
}