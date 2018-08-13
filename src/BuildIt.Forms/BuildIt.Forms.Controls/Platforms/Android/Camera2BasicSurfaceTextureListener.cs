using Android.Views;

namespace BuildIt.Forms.Controls.Platforms.Android
{
    public class Camera2BasicSurfaceTextureListener : Java.Lang.Object, TextureView.ISurfaceTextureListener
    {
        private readonly CameraPreviewControlRenderer owner;

        public Camera2BasicSurfaceTextureListener(CameraPreviewControlRenderer owner)
        {
            if (owner == null)
            {
                throw new System.ArgumentNullException("owner");
            }

            this.owner = owner;
        }

        public void OnSurfaceTextureAvailable(global::Android.Graphics.SurfaceTexture surface, int width, int height)
        {
            owner.OpenCamera(width, height);
        }

        public bool OnSurfaceTextureDestroyed(global::Android.Graphics.SurfaceTexture surface)
        {
            return true;
        }

        public void OnSurfaceTextureSizeChanged(global::Android.Graphics.SurfaceTexture surface, int width, int height)
        {
            owner.ConfigureTransform(width, height);
        }

        public void OnSurfaceTextureUpdated(global::Android.Graphics.SurfaceTexture surface)
        {
        }
    }
}