using Android.Content;
using Android.Hardware;
using Android.Runtime;
using Android.Util;
using Android.Views;
using System;
using System.Collections.Generic;
#pragma warning disable 618

namespace BuildIt.AR.Android.Controls
{
    [Preserve(AllMembers = true)]
    public class CameraPreview : ViewGroup, ISurfaceHolderCallback
    {
        string TAG = "CameraPreview";

        SurfaceView mSurfaceView;
        ISurfaceHolder mHolder;
        IList<Camera.Size> mSupportedPreviewSizes;
        Camera camera;

        public Camera PreviewCamera
        {
            get => camera;
            set
            {
                camera = value;
                if (camera == null)
                {
                    return;
                }

                mSupportedPreviewSizes = PreviewCamera.GetParameters().SupportedPreviewSizes;
                RequestLayout();
            }
        }

        public CameraPreview(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public CameraPreview(Context context) : base(context)
        {
        }

        public CameraPreview(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public CameraPreview(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public CameraPreview(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            // The Surface has been created, acquire the camera and tell it where
            // to draw.
            try
            {
                PreviewCamera?.SetPreviewDisplay(holder);
            }
            catch (Java.IO.IOException exception)
            {
                Log.Error(TAG, "IOException caused by setPreviewDisplay()", exception);
            }
        }

        public void SetPreviewDisplay()
        {
            if (mHolder != null)
            {
                PreviewCamera?.SetPreviewDisplay(mHolder);
            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            // Surface will be destroyed when we return, so stop the preview.
            PreviewCamera?.StopPreview();
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            try
            {
                // We purposely disregard child measurements because act as a
                // wrapper to a SurfaceView that centers the camera preview instead
                // of stretching it.
                int width = ResolveSize(SuggestedMinimumWidth, widthMeasureSpec);
                int height = ResolveSize(SuggestedMinimumHeight, heightMeasureSpec);
                SetMeasuredDimension(width, height);

                if (mSupportedPreviewSizes != null)
                {
                    GetOptimalPreviewSize(mSupportedPreviewSizes, width, height);
                }
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            try
            {
                if (changed && ChildCount > 0)
                {
                    View child = GetChildAt(0);

                    int width = r - l;
                    int height = b - t;

                    child.Layout(0, 0, width, height);
                }
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
        }

        private Camera.Size GetOptimalPreviewSize(IList<Camera.Size> sizes, int w, int h)
        {
            try
            {
                const double ASPECT_TOLERANCE = 0.1;
                double targetRatio = (double)w / h;

                if (sizes == null)
                {
                    return null;
                }

                Camera.Size optimalSize = null;
                double minDiff = Double.MaxValue;

                int targetHeight = h;

                // Try to find an size match aspect ratio and size
                foreach (Camera.Size size in sizes)
                {
                    double ratio = (double)size.Width / size.Height;

                    if (Math.Abs(ratio - targetRatio) > ASPECT_TOLERANCE)
                    {
                        continue;
                    }

                    if (Math.Abs(size.Height - targetHeight) < minDiff)
                    {
                        optimalSize = size;
                        minDiff = Math.Abs(size.Height - targetHeight);
                    }
                }

                // Cannot find the one match the aspect ratio, ignore the requirement
                if (optimalSize == null)
                {
                    minDiff = Double.MaxValue;
                    foreach (Camera.Size size in sizes)
                    {
                        if (Math.Abs(size.Height - targetHeight) < minDiff)
                        {
                            optimalSize = size;
                            minDiff = Math.Abs(size.Height - targetHeight);
                        }
                    }
                }

                return optimalSize;
            }
            catch (Exception ex)
            {
                ex.LogError();
            }

            return null;
        }

        public void SurfaceChanged(ISurfaceHolder holder, global::Android.Graphics.Format format, int w, int h)
        {
            try
            {
                // Now that the size is known, set up the camera parameters and begin
                // the preview.
                // Camera.Parameters parameters = PreviewCamera.GetParameters();
                // parameters.SetPreviewSize(mPreviewSize.Width, mPreviewSize.Height);
                RequestLayout();

                // PreviewCamera.SetParameters(parameters);
                PreviewCamera.StartPreview();
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
        }

        public void InitPreview(Context context)
        {
            try
            {
                mSurfaceView = new SurfaceView(context);
                AddView(mSurfaceView);

                // Install a SurfaceHolder.Callback so we get notified when the
                // underlying surface is created and destroyed.
                mHolder = mSurfaceView.Holder;
                mHolder.AddCallback(this);
                mHolder.SetType(SurfaceType.PushBuffers);
            }
            catch (Exception ex)
            {
                ex.LogError();
            }
        }
    }
}