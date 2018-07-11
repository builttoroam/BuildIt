using AVFoundation;
using BuildIt.Forms.Controls;
using BuildIt.Forms.Controls.Platforms.Ios;
using CoreGraphics;
using Foundation;
using System;
using System.ComponentModel;
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

        /// <inheritdoc />
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (Element == null)
            {
                return;
            }

            if (!Element.IsVisible)
            {
                return;
            }

            try
            {
                SetupUserInterface();
                SetupEventHandlers();
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

            if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName &&
                !isInitialized &&
                Element.IsVisible)
            {
                try
                {
                    SetupUserInterface();
                    SetupEventHandlers();
                    SetupLiveCameraStream();
                    AuthorizeCameraUse();
                    isInitialized = true;
                }
                catch (Exception ex)
                {
                    ex.LogError();
                }
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
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

            base.Dispose(disposing);
        }

        private void SetupUserInterface()
        {
            liveCameraStream = new UIView
            {
                Frame = new CGRect(0f, 0f, NativeView.Bounds.Width, NativeView.Bounds.Height)
            };

            NativeView.Add(liveCameraStream);
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

            var captureDevice = AVCaptureDevice.GetDefaultDevice(AVMediaType.Video);
            ConfigureCameraForDevice(captureDevice);
            captureDeviceInput = AVCaptureDeviceInput.FromDevice(captureDevice);

            var dictionary = new NSMutableDictionary
            {
                [AVVideo.CodecKey] = new NSNumber((int)AVVideoCodec.JPEG)
            };

            stillImageOutput = new AVCaptureStillImageOutput
            {
                OutputSettings = dictionary
            };

            captureSession.AddOutput(stillImageOutput);
            captureSession.AddInput(captureDeviceInput);
            captureSession.StartRunning();
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