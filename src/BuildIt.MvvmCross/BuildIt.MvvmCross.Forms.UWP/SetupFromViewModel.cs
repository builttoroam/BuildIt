using MvvmCross.Core.ViewModels;
using MvvmCross.Forms.Platform;
using System.Collections.Generic;
using System.Reflection;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;

namespace BuildIt.MvvmCross.Forms.UWP
{
    public class SetupFromViewModel<TStartupViewModel, TFormsApplication> 
        : SetupFromApplication<TypedMvxApplication<TStartupViewModel>,TFormsApplication>
       where TStartupViewModel : IMvxViewModel
       where TFormsApplication : MvxFormsApplication, new()
    {
        public SetupFromViewModel(Frame rootFrame, LaunchActivatedEventArgs e) : base(rootFrame, e)
        {
        }

        protected override IEnumerable<Assembly> GetViewModelAssemblies()
        {
            var assembly = typeof(TStartupViewModel).GetTypeInfo().Assembly;
            return new[] { assembly };
        }
    }
}
