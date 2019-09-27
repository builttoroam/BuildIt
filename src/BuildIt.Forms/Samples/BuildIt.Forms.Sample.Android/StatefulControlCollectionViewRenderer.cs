using Android.Content;
using Android.Support.V7.Widget;
using BuildIt.Forms.Controls;
using BuildIt.Forms.Sample.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CollectionView), typeof(StatefulControlCollectionViewRenderer))]

namespace BuildIt.Forms.Sample.Droid
{
    public class StatefulControlCollectionViewRenderer : CollectionViewRenderer
    {
        public StatefulControlCollectionViewRenderer(Context context)
            : base(context)
        {
            var recyclerView = this as RecyclerView;
            recyclerView.Touch += HandleRecyclerViewTouch;
        }

        protected virtual void OnElementChanged(ElementChangedEventArgs<ItemsView> e)
        {
            //if (e.OldElement != null)
            //{
            //    var recyclerView = this as RecyclerView;
            //    recyclerView.Touch -= HandleRecyclerViewTouch;
            //}

            //if (e.NewElement != null)
            //{
            //    var recyclerView = this as RecyclerView;
            //    recyclerView.Touch += HandleRecyclerViewTouch;
            //}
        }

        private void HandleRecyclerViewTouch(object sender, TouchEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"[{nameof(HandleRecyclerViewTouch)}] {e.Event.Action}");
            var statefulControl = (Element as CollectionView)?.Parent as StatefulControl;
            statefulControl?.HandleCollectionViewTouchEvents(e.Event);
        }
    }
}