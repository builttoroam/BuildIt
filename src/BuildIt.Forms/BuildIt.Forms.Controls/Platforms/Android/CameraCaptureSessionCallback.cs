using System.Linq;
using Android.Hardware.Camera2;
using Android.OS;
using BuildIt.Forms.Controls.Platforms.Android.Extensions;

namespace BuildIt.Forms.Controls.Platforms.Android
{
    public class CameraCaptureSessionCallback : CameraCaptureSession.StateCallback
    {
        private readonly CameraPreviewControlRenderer owner;

        public CameraCaptureSessionCallback(CameraPreviewControlRenderer owner)
        {
            this.owner = owner;
        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            // TODO: callback when camera couldn't be configured
        }

        public override void OnConfigured(CameraCaptureSession session)
        {
            // The camera is already closed
            if (owner.CameraDevice == null)
            {
                return;
            }

            // When the session is ready, we start displaying the preview.
            owner.CaptureSession = session;
            try
            {
                var defaultFocusMode = CameraFocusMode.Auto;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    var supportedFocusModes = owner.RetrieveCamera2SupportedFocusModes();
                    if (supportedFocusModes.Contains(owner.DesiredFocusMode))
                    {
                        defaultFocusMode = owner.DesiredFocusMode;
                    }
                    else
                    {
                        defaultFocusMode = supportedFocusModes.OrderBy(m => (int)m)
                                                              .LastOrDefault();
                    }
                }

                owner.PreviewRequestBuilder.Set(CaptureRequest.ControlAfMode, (int)defaultFocusMode.ToPlatformFocusMode());
                // Flash is automatically enabled when necessary.
                owner.SetAutoFlash(owner.PreviewRequestBuilder);

                // Finally, we start displaying the camera preview.
                owner.PreviewRequest = owner.PreviewRequestBuilder.Build();
                owner.CaptureSession.SetRepeatingRequest(owner.PreviewRequest, owner.CaptureCallback, owner.BackgroundHandler);
            }
            catch (CameraAccessException e)
            {
                e.PrintStackTrace();
            }
        }
    }
}