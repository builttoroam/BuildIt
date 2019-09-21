using Android.Hardware;
using Android.Hardware.Camera2;
using BuildIt.AR.Android.Controls;
using Java.Lang;
#pragma warning disable 618

namespace BuildIt.AR.Android.Utilities
{
    public class CameraFeedUtility
    {
        private readonly CameraPreview cameraPreview;
        private Camera camera;

        public CameraFeedUtility(CameraPreview cameraPreview)
        {
            this.cameraPreview = cameraPreview;
        }

        public void StartPreview()
        {
            try
            {
                var numberOfCameras = Camera.NumberOfCameras;
                int? rearFacingCameraId = null;
                // Find the ID of the default camera
                Camera.CameraInfo cameraInfo = new Camera.CameraInfo();
                for (int i = 0; i < numberOfCameras; i++)
                {
                    Camera.GetCameraInfo(i, cameraInfo);
                    if (cameraInfo.Facing == CameraFacing.Back)
                    {
                        rearFacingCameraId = i;
                    }
                }

                if (rearFacingCameraId.HasValue)
                {
                    camera = Camera.Open(rearFacingCameraId.Value);
                    if (cameraPreview != null)
                    {
                        cameraPreview.PreviewCamera = camera;
                    }
                }
            }
            catch (CameraAccessException cex)
            {
                cex.LogError();
            }
            catch (NullPointerException nex)
            {
                nex.LogError();
            }
            catch (System.Exception ex)
            {
                ex.LogError();
            }
        }

        public void CleanUpCamera()
        {
            if (camera != null)
            {
                camera.StopPreview();
                if (cameraPreview != null)
                {
                    cameraPreview.PreviewCamera = null;
                }

                camera.Release();
                camera = null;
            }
        }

        public void UpdatePreviewRotation(Rotation rotation)
        {
            var angle = 0;
            switch (rotation)
            {
                case Rotation.Rotation0:
                    angle = 90;
                    break;

                case Rotation.Rotation90:
                    break;

                case Rotation.Rotation180:
                    angle = 270;
                    break;

                case Rotation.Rotation270:
                    angle = 180;
                    break;
            }

            camera?.SetDisplayOrientation(angle);
        }
    }
}