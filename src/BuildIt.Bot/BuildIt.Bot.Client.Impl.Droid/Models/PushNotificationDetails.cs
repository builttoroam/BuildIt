using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BuildIt.Bot.Client.Impl.Droid.Models
{
    public class PushNotificationDetails
    {
        public int? SmallIcon { get; set; }

        public ActivityFlags ActivityFlags { get; set; } = ActivityFlags.ClearTop | ActivityFlags.SingleTop;
    }
}