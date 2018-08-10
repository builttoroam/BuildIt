using Android.Hardware.Camera2;


namespace BuildIt.Forms.Controls.Platforms.Android
{
    public class CameraCaptureSessionCallback : CameraCaptureSession.StateCallback
    {
        private readonly CameraPreviewControlRenderer owner;

        public CameraCaptureSessionCallback(CameraPreviewControlRenderer owner)
        {
            if (owner == null)
                throw new System.ArgumentNullException("owner");
            this.owner = owner;
        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            //owner.ShowToast("Failed");
        }

        public override void OnConfigured(CameraCaptureSession session)
        {
            // The camera is already closed
            if (null == owner.cameraDevice)
            {
                return;
            }

            // When the session is ready, we start displaying the preview.
            owner.captureSession = session;
            try
            {
                // default to auto
                owner.previewRequestBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAFMode.Auto);
                // Flash is automatically enabled when necessary.
                owner.SetAutoFlash(owner.previewRequestBuilder);

                // Finally, we start displaying the camera preview.
                owner.previewRequest = owner.previewRequestBuilder.Build();
                owner.captureSession.SetRepeatingRequest(owner.previewRequest, owner.captureCallback, owner.backgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }
    }
}
