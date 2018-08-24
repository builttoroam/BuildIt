using Android.Content;
using Android.Util;
using Android.Views;
using System;
using Xamarin.Forms;

namespace BuildIt.Forms.Controls.Platforms.Android
{
    public class AutoFitTextureView : TextureView
    {
        private int ratioWidth = 0;
        private int ratioHeight = 0;
        private Aspect aspect;

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

        public void SetAspectRatio(Aspect aspect, int width, int height)
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
                case Aspect.AspectFit:
                    if (width < (float)height * ratioWidth / (float)ratioHeight)
                    {
                        SetMeasuredDimension(width, width * ratioHeight / ratioWidth);
                    }
                    else
                    {
                        SetMeasuredDimension(height * ratioWidth / ratioHeight, height);
                    }

                    break;

                case Aspect.AspectFill:
                    var previewAspectRatio = (float)ratioWidth / ratioHeight;
                    if (previewAspectRatio > 1)
                    {
                        // width > height, so keep the height but scale the width to meet aspect ratio
                        SetMeasuredDimension((int)(width * previewAspectRatio), height);
                    }
                    else if (previewAspectRatio < 1 && previewAspectRatio != 0)
                    {
                        // width < height
                        SetMeasuredDimension(width, (int)(height / previewAspectRatio));
                    }
                    else
                    {
                        SetMeasuredDimension(width, height);
                    }

                    break;

                case Aspect.Fill:
                    SetMeasuredDimension(width, height);
                    break;
            }
        }
    }
}