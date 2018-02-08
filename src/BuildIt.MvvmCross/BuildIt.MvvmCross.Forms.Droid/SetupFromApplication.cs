using Android.Content;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Views;
using MvvmCross.Forms.Droid.Platform;
using MvvmCross.Forms.Droid.Views;
using MvvmCross.Forms.Platform;
using MvvmCross.Forms.Views;
using MvvmCross.Platform;
using MvvmCross.Platform.Droid;
using MvvmCross.Platform.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace BuildIt.MvvmCross.Forms.Droid
{
    public interface IDroidExecutable
    {
        void UpdateExecutable(Type referenceType);
    }

    public class SetupFromApplication<TApplication, TFormsApplication> : MvxFormsAndroidSetup, IDroidExecutable
        where TApplication : IMvxApplication, new()
        where TFormsApplication : MvxFormsApplication, new()
    {
        public SetupFromApplication(Context applicationContext) : base(applicationContext)
        {
        }

        private string executableNamespace;
        public override string ExecutableNamespace => executableNamespace;

        private Assembly executableAssembly;
        public override Assembly ExecutableAssembly => executableAssembly;

        public void UpdateExecutable(Type referenceType)
        {
            executableNamespace = referenceType.Namespace;
            executableAssembly = referenceType.Assembly;

            base.InitializeBindingBuilder();
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

        protected override IMvxAndroidViewPresenter CreateViewPresenter()
        {
            var viewAssemblies = GetViewAssemblies();
            var presenter = new CustomMvxFormsAndroidViewPresenter(viewAssemblies, FormsApplication);
            Mvx.RegisterSingleton<IMvxFormsViewPresenter>(presenter);
            return presenter;
        }

        protected override void InitializeBindingBuilder()
        {
            // Do nothing - this isn't required for regular forms binding
        }
    }

    public class CustomMvxFormsAndroidViewPresenter : MvxFormsAndroidViewPresenter
    {
        public CustomMvxFormsAndroidViewPresenter(IEnumerable<Assembly> androidViewAssemblies, MvxFormsApplication formsApplication) : base(androidViewAssemblies, formsApplication)
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
