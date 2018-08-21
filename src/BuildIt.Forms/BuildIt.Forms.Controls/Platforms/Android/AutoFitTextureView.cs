using Android.Content;
using Android.Util;
using Android.Views;
using System;

namespace BuildIt.Forms.Controls.Platforms.Android
{
    public class AutoFitTextureView : TextureView
    {
        private int ratioWidth = 0;
        private int ratioHeight = 0;
        private CameraPreviewAspect aspect;

        public AutoFitTextureView(Context context)
            : this(context, null)
        {
        }

        public AutoFitTextureView(Context context, IAttributeSet attrs)
            : this(context, attrs, 0)
        {
        }

        public AutoFitTextureView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
        }

        public void SetAspectRatio(CameraPreviewAspect aspect, int width, int height)
        {
            if (width == 0 || height == 0)
            {
                throw new ArgumentException("Size cannot be negative.");
            }

            this.aspect = aspect;
            ratioWidth = width;
            ratioHeight = height;
            RequestLayout();
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            int width = MeasureSpec.GetSize(widthMeasureSpec);
            int height = MeasureSpec.GetSize(heightMeasureSpec);
            if (ratioWidth == 0 || ratioHeight == 0)
            {
                SetMeasuredDimension(width, height);
                return;
            }

            switch (aspect)
            {
                case CameraPreviewAspect.AspectFit:
                    if (width < (float)height * ratioWidth / (float)ratioHeight)
                    {
                        System.Diagnostics.Debug.WriteLine($"bui:2 {width}, {width * ratioHeight / ratioWidth}");
                        SetMeasuredDimension(width, width * ratioHeight / ratioWidth);
                    }
                    else
                    {
                        SetMeasuredDimension(height * ratioWidth / ratioHeight, height);
                    }

                    break;

                case CameraPreviewAspect.Fill:
                    SetMeasuredDimension(width, height);
                    break;
            }
        }
    }
}