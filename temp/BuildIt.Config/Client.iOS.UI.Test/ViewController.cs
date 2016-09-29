using System;
using Client.Core.ViewModels;
using Client.iOS.Impl;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using MvvmCross.Platform;
using UIKit;

namespace Client.iOS.UI.Test
{
    [MvxFromStoryboard("Main")]
    public partial class ViewController : MvxViewController<MainViewModel>
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var set = this.CreateBindingSet<ViewController, MainViewModel>();
            set.Bind(GetConfigButton).To(vm => vm.GetAppConfigCommand);
            set.Apply();
        }
    }
}