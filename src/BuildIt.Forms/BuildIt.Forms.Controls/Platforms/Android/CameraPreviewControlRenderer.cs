using Android.App;
using Android.Graphics;
using Android.Hardware;
using Android.Views;
using Android.Widget;
using BuildIt.Forms.Controls;
using BuildIt.Forms.Controls.Platforms.Android;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Context = Android.Content.Context;

[assembly: ExportRenderer(typeof(CameraPreviewControl), typeof(CameraPreviewControlRenderer))]
namespace BuildIt.Forms.Controls.Platforms.Android
{
    /// <summary>
    /// Custom Renderer for <see cref="CameraPreviewControl"/>
    /// </summary>
    public class CameraPreviewControlRenderer : FrameRenderer, TextureView.ISurfaceTextureListener
    {
        private global::Android.Hardware.Camera camera;
        private global::Android.Views.View view;

        private Activity activity;
        private CameraFacing cameraType;
        private TextureView textureView;
        private SurfaceTexture surfaceTexture;

        private CameraPreviewControl cameraPreviewControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraPreviewControlRenderer"/> class.
        /// </summary>
        /// <param name="context"><see cref="Context"/></param>
        public CameraPreviewControlRenderer(Context context)
            : base(context)
        {
        }

        /// <inheritdoc />
        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
        {
            camera = global::Android.Hardware.Camera.Open((int)cameraType);
            textureView.LayoutParameters = new FrameLayout.LayoutParams(width, height);
            surfaceTexture = surface;

            camera.SetPreviewTexture(surface);
            PrepareAndStartCamera();
        }

        /// <inheritdoc />
        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            camera.StopPreview();
            camera.Release();
            return true;
        }

        /// <inheritdoc />
        public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
        {
            PrepareAndStartCamera();
        }

        /// <inheritdoc />
        public void OnSurfaceTextureUpdated(SurfaceTexture surface)
        {
        }

        /// <inheritdoc />
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            var cpc = Element as CameraPreviewControl;
            if (cpc == null)
            {
                return;
            }

            cameraPreviewControl = cpc;

            try
            {
                SetupUserInterface();
                SetupEventHandlers();
                AddView(view);
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
        }

        /// <inheritdoc />
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == CameraPreviewControl.PreferredCameraProperty.PropertyName)
            {
                SwitchCameraIfNecessary();
            }
        }

        /// <inheritdoc />
        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
            var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);

            view.Measure(msw, msh);
            view.Layout(0, 0, r - l, b - t);
        }

        private static CameraFacing ToCameraFacing(CameraPreviewControl.CameraPreference cameraPreference)
        {
            return cameraPreference == CameraPreviewControl.CameraPreference.Back
                ? CameraFacing.Back
                : CameraFacing.Front;
        }

        private void SwitchCameraIfNecessary()
        {
            var newCameraType = ToCameraFacing(cameraPreviewControl.PreferredCamera);
            if (newCameraType == cameraType)
            {
                return;
            }

            cameraType = newCameraType;

            camera.StopPreview();
            camera.Release();
            camera = global::Android.Hardware.Camera.Open((int)cameraType);
            camera.SetPreviewTexture(surfaceTexture);
            PrepareAndStartCamera();
        }

        private void SetupUserInterface()
        {
            activity = Context as Activity;
            if (activity == null)
            {
                return;
            }

            view = activity.LayoutInflater.Inflate(Resource.Layout.CameraLayout, this, false);
            cameraType = ToCameraFacing(cameraPreviewControl.PreferredCamera);

            textureView = view.FindViewById<TextureView>(Resource.Id.textureView);
            textureView.SurfaceTextureListener = this;
        }

        private void SetupEventHandlers()
        {
        }

        private void PrepareAndStartCamera()
        {
            camera.StopPreview();

            var display = activity.WindowManager.DefaultDisplay;
            if (display.Rotation == SurfaceOrientation.Rotation0)
            {
                camera.SetDisplayOrientation(90);
            }

            if (display.Rotation == SurfaceOrientation.Rotation270)
            {
                camera.SetDisplayOrientation(180);
            }

            camera.StartPreview();
        }
    }
}