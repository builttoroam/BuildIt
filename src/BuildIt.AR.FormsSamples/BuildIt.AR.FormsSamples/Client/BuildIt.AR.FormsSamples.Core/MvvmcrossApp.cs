using BuildIt.AR.FormsSamples.Core.ViewModels;
using BuildIt.AR.FormsSamples.Models;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;
using System.Reflection;
using BuildIt.AR.FormsSamples.Client;
using BuildIt.AR.FormsSamples.Core.Services.Interfaces;

namespace BuildIt.AR.FormsSamples.Core
{
    public class MvvmcrossApp : MvxApplication
    {
        public override void Initialize()
        {


            //var uiLibrary = typeof(HomeViewModel).GetTypeInfo().Assembly;

            CreatableTypes().EndingWith("Service")
                            .AsInterfaces()
                            .RegisterAsLazySingleton();

            var config = Mvx.Resolve<IConfigurationService>();
            
            Mvx.LazyConstructAndRegisterSingleton<IAppServiceAPI>(() => new AppServiceAPI(config.CurrentConfiguration.BaseUri));

            RegisterAppStart<HomeViewModel>();
        }
    }
}
