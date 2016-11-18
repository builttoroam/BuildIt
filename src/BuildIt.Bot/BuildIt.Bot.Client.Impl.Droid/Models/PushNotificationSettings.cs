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
using BuildIt.Bot.Client.Models;

namespace BuildIt.Bot.Client.Impl.Droid.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class PushNotificationSettings
    {
        /// <summary>
        /// 
        /// </summary>
        public int? SmallIcon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityFlags ActivityFlags { get; set; } = ActivityFlags.ClearTop | ActivityFlags.SingleTop;

        /// <summary>
        /// 
        /// </summary>        
        public string GoogleApiConsoleAppProjectNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public EndpointRouteDetails EndpointRouteDetails { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Type MainActivityType { get; set; }
    }
}