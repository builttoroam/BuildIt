using Foundation;
using System;
using BuildItConfigSample.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using UIKit;

namespace BuildItConfigSample.iOS
{
    public partial class FirstViewController : MvxViewController<FirstViewModel>
    {
        public FirstViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var set = this.CreateBindingSet<FirstViewController, FirstViewModel>();
            set.Bind(GetConfigButton).To(vm => vm.GetAppConfigCommand);
            set.Apply();
        }
    }
}