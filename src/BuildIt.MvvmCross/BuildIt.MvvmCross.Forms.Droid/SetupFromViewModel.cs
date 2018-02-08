using Android.Content;
using MvvmCross.Core.ViewModels;
using MvvmCross.Forms.Platform;
using System.Collections.Generic;
using System.Reflection;

namespace BuildIt.MvvmCross.Forms.Droid
{
    public class SetupFromViewModel<TStartupViewModel, TFormsApplication> 
        : SetupFromApplication<TypedMvxApplication<TStartupViewModel>,TFormsApplication>
       where TStartupViewModel : IMvxViewModel
       where TFormsApplication : MvxFormsApplication, new()
    {
        public SetupFromViewModel(Context applicationContext) : base(applicationContext)
        {
        }

        protected override IEnumerable<Assembly> GetViewModelAssemblies()
        {
            var assembly = typeof(TStartupViewModel).GetTypeInfo().Assembly;
            return new[] { assembly };
        }
    }
}
