using Android.App;
using Android.Hardware.Camera2;

namespace BuildIt.Forms.Controls.Platforms.Android
{
    public class CameraStateListener : CameraDevice.StateCallback
    {
        private readonly CameraPreviewControlRenderer owner;

        public CameraStateListener(CameraPreviewControlRenderer owner)
        {
            if (owner == null)
                throw new System.ArgumentNullException("owner");
            this.owner = owner;
        }

        public override void OnOpened(CameraDevice cameraDevice)
        {
            // This method is called when the camera is opened.  We start camera preview here.
            owner.CameraOpenCloseLock.Release();
            owner.cameraDevice = cameraDevice;
            owner.CreateCameraPreviewSession();
        }

        public override void OnDisconnected(CameraDevice cameraDevice)
        {
            owner.CameraOpenCloseLock.Release();
            cameraDevice.Close();
            owner.cameraDevice = null;
        }

        public override void OnError(CameraDevice cameraDevice, CameraError error)
        {
            owner.CameraOpenCloseLock.Release();
            cameraDevice.Close();
            owner.cameraDevice = null;
            if (owner == null)
            {
                return;
            }

            Activity activity = owner.Activity;
            if (activity != null)
            {
                activity.Finish();
            }
        }
    }
}
