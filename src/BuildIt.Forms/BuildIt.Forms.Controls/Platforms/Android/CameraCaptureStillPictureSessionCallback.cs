using Android.Hardware.Camera2;
using Android.Util;

namespace BuildIt.Forms.Controls.Platforms.Android
{
    public class CameraCaptureStillPictureSessionCallback : CameraCaptureSession.CaptureCallback
    {
        private static readonly string TAG = "CameraCaptureStillPictureSessionCallback";

        private readonly CameraPreviewControlRenderer owner;

        public CameraCaptureStillPictureSessionCallback(CameraPreviewControlRenderer owner)
        {
            this.owner = owner;
        }

        public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
        {
            owner.UnlockFocus();
        }
    }
}
