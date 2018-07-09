using Android.Views;

namespace BuildIt.AR.FormsSamples.Android.Listeners
{
    public class Camera2BasicSurfaceTextureListener : Java.Lang.Object, TextureView.ISurfaceTextureListener
    {
        public Camera2BasicFragment Owner { get; set; }

        public Camera2BasicSurfaceTextureListener(Camera2BasicFragment owner)
        {
            Owner = owner;
        }

        public void OnSurfaceTextureAvailable(global::Android.Graphics.SurfaceTexture surface, int width, int height)
        {
            Owner.OpenCamera(width, height);
        }

        public bool OnSurfaceTextureDestroyed(global::Android.Graphics.SurfaceTexture surface)
        {
            return true;
        }

        public void OnSurfaceTextureSizeChanged(global::Android.Graphics.SurfaceTexture surface, int width, int height)
        {
            Owner.ConfigureTransform(width, height);
        }

        public void OnSurfaceTextureUpdated(global::Android.Graphics.SurfaceTexture surface)
        {
        }
    }
}