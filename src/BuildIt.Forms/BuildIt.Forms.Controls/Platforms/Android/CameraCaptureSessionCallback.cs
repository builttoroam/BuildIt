using Android.Hardware.Camera2;

namespace BuildIt.Forms.Controls.Platforms.Android
{
    public class CameraCaptureSessionCallback : CameraCaptureSession.StateCallback
    {
        private readonly CameraPreviewControlRenderer owner;

        public CameraCaptureSessionCallback(CameraPreviewControlRenderer owner)
        {
            if (owner == null)
            {
                throw new System.ArgumentNullException("owner");
            }

            this.owner = owner;
        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            //owner.ShowToast("Failed");
        }

        public override void OnConfigured(CameraCaptureSession session)
        {
            // The camera is already closed
            if (owner.cameraDevice == null)
            {
                return;
            }

            // When the session is ready, we start displaying the preview.
            owner.captureSession = session;
            try
            {
                // default to auto
                owner.PreviewRequestBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.Auto);

                // Flash is automatically enabled when necessary.
                owner.SetAutoFlash(owner.PreviewRequestBuilder);

                // Finally, we start displaying the camera preview.
                owner.PreviewRequest = owner.PreviewRequestBuilder.Build();
                owner.captureSession.SetRepeatingRequest(owner.PreviewRequest, owner.CaptureCallback, owner.backgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }
    }
}