using MvvmCross.Core.ViewModels;
using MvvmCross.Forms.Platform;
using MvvmCross.Forms.Uwp;
using MvvmCross.Forms.Uwp.Presenters;
using MvvmCross.Forms.Views;
using MvvmCross.Platform;
using MvvmCross.Platform.Logging;
using MvvmCross.Platform.Platform;
using MvvmCross.Uwp.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;

namespace BuildIt.MvvmCross.Forms.UWP
{
    public class SetupFromApplication<TApplication, TFormsApplication> : MvxFormsWindowsSetup
        where TApplication : IMvxApplication, new()
        where TFormsApplication : MvxFormsApplication, new()
    {
        public SetupFromApplication(Frame rootFrame, LaunchActivatedEventArgs e) : base(rootFrame, e)
        {
        }

        protected override MvxLogProviderType GetDefaultLogProviderType() => MvxLogProviderType.None;

        protected override IMvxLogProvider CreateLogProvider() => new EmptyVoidLogProvider();

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

        protected override IMvxWindowsViewPresenter CreateViewPresenter(IMvxWindowsFrame rootFrame)
        {
            var presenter = new CustomMvxFormsUwpViewPresenter(rootFrame, FormsApplication);
            Mvx.RegisterSingleton<IMvxFormsViewPresenter>(presenter);
            return presenter;
        }
    }

    public class CustomMvxFormsUwpViewPresenter : MvxFormsUwpViewPresenter
    {
        public CustomMvxFormsUwpViewPresenter(IMvxWindowsFrame rootFrame) : base(rootFrame)
        {
        }

        public CustomMvxFormsUwpViewPresenter(IMvxWindowsFrame rootFrame, MvxFormsApplication formsApplication) : base(rootFrame, formsApplication)
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
