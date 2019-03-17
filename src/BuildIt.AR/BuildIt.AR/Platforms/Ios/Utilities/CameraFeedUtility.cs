
using System.Collections.Generic;
using AVFoundation;
using Foundation;
using UIKit;

namespace BuildIt.AR.iOS.Utilities
{
    public class CameraFeedUtility
    {
        private AVCaptureSession session;
        private AVCaptureVideoPreviewLayer previewLayer;
        private readonly UIView rootView;
        private readonly IDictionary<UIInterfaceOrientation, AVCaptureVideoOrientation> configDicByRotationChanged = new Dictionary<UIInterfaceOrientation, AVCaptureVideoOrientation>
        {
            {UIInterfaceOrientation.LandscapeLeft, AVCaptureVideoOrientation.LandscapeLeft},
            {UIInterfaceOrientation.LandscapeRight, AVCaptureVideoOrientation.LandscapeRight},
            {UIInterfaceOrientation.Portrait, AVCaptureVideoOrientation.Portrait},
            {UIInterfaceOrientation.PortraitUpsideDown, AVCaptureVideoOrientation.PortraitUpsideDown},
            {UIInterfaceOrientation.Unknown, AVCaptureVideoOrientation.Portrait}
        };
        private readonly UIView cameraView;

        public CameraFeedUtility(UIView rootView, UIView cameraView)
        {
            this.rootView = rootView;
            this.cameraView = cameraView;
        }

        public void InitAndStartCamera()
        {
            session = new AVCaptureSession
            {
                SessionPreset = AVCaptureSession.PresetMedium
            };
            var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);

            NSError error;
            var videoInput = AVCaptureDeviceInput.FromDevice(captureDevice, out error);

            if (videoInput == null || !session.CanAddInput(videoInput)) return;
            session.AddInput(videoInput);
            previewLayer = new AVCaptureVideoPreviewLayer(session) { Frame = rootView.Bounds };
            previewLayer.Connection.VideoOrientation = configDicByRotationChanged[UIApplication.SharedApplication.StatusBarOrientation];
            previewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
            cameraView.Layer.AddSublayer(previewLayer);
            session.StartRunning();
        }

        public void StopCamera()
        {
            session?.StopRunning();
        }

        public void UpdatePreviewRotation(UIInterfaceOrientation orientation)
        {
            //Re-size camera feed based on orientation
            if (previewLayer == null) return;
            previewLayer.Connection.VideoOrientation = configDicByRotationChanged[orientation];
            previewLayer.Frame = rootView.Bounds;
        }
    }
}