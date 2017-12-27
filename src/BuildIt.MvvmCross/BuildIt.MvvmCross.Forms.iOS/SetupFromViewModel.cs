using MvvmCross.Core.ViewModels;
using MvvmCross.Forms.Platform;
using MvvmCross.iOS.Platform;
using System.Collections.Generic;
using System.Reflection;
using UIKit;

namespace BuildIt.MvvmCross.Forms.iOS
{
    public class SetupFromViewModel<TStartupViewModel, TFormsApplication> 
        : SetupFromApplication<TypedMvxApplication<TStartupViewModel>,TFormsApplication>
       where TStartupViewModel : IMvxViewModel
       where TFormsApplication : MvxFormsApplication, new()
    {
        public SetupFromViewModel(IMvxApplicationDelegate applicationDelegate, UIWindow window)
            : base(applicationDelegate, window)
        { 
        }

        protected override IEnumerable<Assembly> GetViewModelAssemblies()
        {
            var assembly = typeof(TStartupViewModel).GetTypeInfo().Assembly;
            return new[] { assembly };
        }
    }
}
