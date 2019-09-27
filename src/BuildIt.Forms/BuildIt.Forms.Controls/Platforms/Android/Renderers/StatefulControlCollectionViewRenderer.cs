using System;
using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.Views;
using BuildIt.Forms.Controls;
using BuildIt.Forms.Controls.Platforms.Android.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly: ExportRenderer(typeof(CollectionView), typeof(StatefulControlCollectionViewRenderer))]
namespace BuildIt.Forms.Controls.Platforms.Android.Renderers
{
    public class StatefulControlCollectionViewRenderer : CollectionViewRenderer
    {
        public StatefulControlCollectionViewRenderer(Context context)
            : base(context)
        {
        }
    }
}