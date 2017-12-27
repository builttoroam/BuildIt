using MvvmCross.Core.ViewModels;
using MvvmCross.Forms.iOS;
using MvvmCross.Forms.iOS.Presenters;
using MvvmCross.Forms.Platform;
using MvvmCross.Forms.Views;
using MvvmCross.iOS.Platform;
using MvvmCross.iOS.Views.Presenters;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UIKit;

namespace BuildIt.MvvmCross.Forms.iOS
{
    public class SetupFromApplication<TApplication, TFormsApplication> : MvxFormsIosSetup
        where TApplication : IMvxApplication, new()
        where TFormsApplication : MvxFormsApplication, new()
    {
        public SetupFromApplication(IMvxApplicationDelegate applicationDelegate, UIWindow window)
            : base(applicationDelegate, window)
        {
        }

        protected override IEnumerable<Assembly> GetViewAssemblies()
        {
            var assemblies = new List<Assembly>(
                base.GetViewAssemblies().Union(new[] { typeof(TFormsApplication).GetTypeInfo().Assembly }));
            return assemblies;
        }

        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }

        protected override MvxFormsApplication CreateFormsApplication() => new TFormsApplication();

        protected override IMvxApplication CreateApp() => new TApplication();


        protected override void InitializeViewModelTypeFinder()
        {
            base.InitializeViewModelTypeFinder();

            var viewModelByNameLookup = Mvx.Resolve<IMvxViewModelByNameLookup>();

            var nameMappingStrategy = CreateViewToViewModelNaming();
            var finder = new CustomMvxViewModelViewTypeFinder(viewModelByNameLookup, nameMappingStrategy);
            Mvx.RegisterSingleton<IMvxViewModelTypeFinder>(finder);
        }

        protected override IMvxIosViewPresenter CreatePresenter()
        {
            var presenter = new CustomMvxFormsIosViewPresenter(ApplicationDelegate, Window, FormsApplication);
            Mvx.RegisterSingleton<IMvxFormsViewPresenter>(presenter);
            return presenter;
        }
    }

    public class CustomMvxFormsIosViewPresenter : MvxFormsIosViewPresenter
    {
        public CustomMvxFormsIosViewPresenter(IUIApplicationDelegate applicationDelegate, UIWindow window, MvxFormsApplication formsApplication) : base(applicationDelegate, window, formsApplication)
        {
        }


        private IMvxFormsPagePresenter formsPagePresenter;
        public override IMvxFormsPagePresenter FormsPagePresenter
        {
            get
            {
                if (formsPagePresenter == null)
                {
                    formsPagePresenter = new CustomMvxFormsPagePresenter(FormsApplication, ViewsContainer, ViewModelTypeFinder, attributeTypesToActionsDictionary: AttributeTypesToActionsDictionary);
                    formsPagePresenter.ClosePlatformViews = ClosePlatformViews;
                    Mvx.RegisterSingleton(formsPagePresenter);
                }
                return formsPagePresenter;
            }
            set
            {
                formsPagePresenter = value;
            }
        }
    }

    public class DebugTrace : IMvxTrace
    {
        public void Trace(MvxTraceLevel level, string tag, Func<string> message)
        {
            Debug.WriteLine($"{tag} : {level} : {message()}");
        }

        public void Trace(MvxTraceLevel level, string tag, string message)
        {
            Debug.WriteLine($"{tag} : {level} : {message}");
        }

        public void Trace(MvxTraceLevel level, string tag, string message, params object[] args)
        {
            try
            {
                Debug.WriteLine($"{tag} : {level} : {message}, {args}");
            }
            catch (FormatException)
            {
                Trace(MvxTraceLevel.Error, tag, "Exception during trace of {0} {1}", level, message);
            }
        }
    }
}
