using System;
using System.Collections.Generic;
using System.Text;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.Widget;
using Xamarin.Forms.Platform.Android;

namespace BuildIt.Forms.Controls.Platforms.Android
{
    public class PullToRefreshControl : SwipeRefreshLayout, SwipeRefreshLayout.IOnRefreshListener
    {
        public PullToRefreshControl(Context context)
            : base(context)
        {
            // SetBackgroundColor(Color.Aqua);
        }

        public void OnRefresh()
        {
            // throw new NotImplementedException();
        }

        public override bool CanChildScrollUp()
        {
            return false;
        }

        
    }
}
