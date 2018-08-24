using Android.Content;
using Android.Util;
using Android.Views;
using System;
using Xamarin.Forms;

namespace BuildIt.Forms.Controls.Platforms.Android
{
    public class AutoFitTextureView : TextureView
    {
        private double ratioWidth = 0;
        private double ratioHeight = 0;
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
            var width = (double)MeasureSpec.GetSize(widthMeasureSpec);
            var height = (double)MeasureSpec.GetSize(heightMeasureSpec);
            if (ratioWidth == 0 || ratioHeight == 0)
            {
                SetMeasuredDimension((int)width, (int)height);
                return;
            }

            var previewAspectRatio = (double)ratioWidth / ratioHeight;
            switch (aspect)
            {
                case Aspect.AspectFit:
                    if (width < height * previewAspectRatio)
                    {
                        SetMeasuredDimension((int)width, (int)(width / previewAspectRatio));
                    }
                    else
                    {
                        SetMeasuredDimension((int)(height * previewAspectRatio), (int)height);
                    }

                    break;

                case Aspect.AspectFill:
                    if (previewAspectRatio > 1)
                    {
                        // width > height, so keep the height but scale the width to meet aspect ratio
                        SetMeasuredDimension((int)(width * previewAspectRatio), (int)height);
                    }
                    else if (previewAspectRatio < 1 && previewAspectRatio != 0)
                    {
                        // width < height
                        SetMeasuredDimension((int)width, (int)(height / previewAspectRatio));
                    }
                    else
                    {
                        SetMeasuredDimension((int)width, (int)height);
                    }

                    break;

                case Aspect.Fill:
                    SetMeasuredDimension((int)width, (int)height);
                    break;
            }
        }
    }
}