using Android.Hardware.Camera2;

namespace BuildIt.Forms.Controls.Platforms.Android
{
    public class CameraStateListener : CameraDevice.StateCallback
    {
        private readonly CameraPreviewControlRenderer owner;

        public CameraStateListener(CameraPreviewControlRenderer owner)
        {
            this.owner = owner;
        }

        public override void OnOpened(CameraDevice cameraDevice)
        {
            // This method is called when the camera is opened.  We start camera preview here.
            owner.CameraOpenCloseLock.Release();
            owner.CameraDevice = cameraDevice;
            owner.CreateCameraPreviewSession();
        }

        public override void OnDisconnected(CameraDevice cameraDevice)
        {
            owner.CameraOpenCloseLock.Release();
            cameraDevice.Close();
            owner.CameraDevice = null;
        }

        public override void OnError(CameraDevice cameraDevice, CameraError error)
        {
            cameraDevice.Close();
            if (owner == null)
            {
                return;
            }
            owner.CameraOpenCloseLock.Release();
            owner.CameraDevice = null;
            owner.RaiseErrorOpening();
        }
    }
}