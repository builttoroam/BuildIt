using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuildIt.MvvmCross.Forms
{
    public class TypedMvxApplication<TStartupViewModel> : MvxApplication
        where TStartupViewModel : IMvxViewModel
    {
        public override void Initialize()
        {
            RegisterNavigationServiceAppStart<TStartupViewModel>();
        }
    }
}
