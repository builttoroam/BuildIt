using System;
using BuildIt.Client.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using UIKit;

namespace BuildIt.Client.iOS.UI.Test.View
{
    public partial class ConfigViewController : MvxViewController<MainViewModel>
    {
        public ConfigViewController() : base("ConfigViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var set = this.CreateBindingSet<ConfigViewController, MainViewModel>();
            set.Bind(GetConfigButton).To(vm => vm.GetAppConfigCommand);
            set.Apply();
        }
    }
}