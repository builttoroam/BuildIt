using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using BuildItArSample.Core;

namespace BuildItArSample.Android.Controls
{
    public class ArMarker : LinearLayout
    {
        private TextView distanceText;
        public TextView DistanceText => distanceText ?? (distanceText = FindViewById<TextView>(Resource.Id.distanceText));

        private TextView titleText;
        public TextView TitleText => titleText ?? (titleText = FindViewById<TextView>(Resource.Id.titleText));

        public POI POI { get; private set; }

        public ArMarker(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Init();
        }

        public ArMarker(Context context, POI poi) : base(context)
        {
            Init();
            POI = poi;
        }

        public ArMarker(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init();
        }

        public ArMarker(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init();
        }

        public ArMarker(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            Init();
        }

        private void Init()
        {
            Inflate(Context, Resource.Layout.ArMarker, this);
        }
    }
}