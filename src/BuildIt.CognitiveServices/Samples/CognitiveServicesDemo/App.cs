using CognitiveServicesDemo.Service.Interfaces;
using CognitiveServicesDemo.ViewModels;

namespace CognitiveServicesDemo
{
    public class App : MvvmCross.Core.ViewModels.MvxApplication
    {
        public override void Initialize()
        {
            RegisterAppStart<MainViewModel>();
            //Mvx.LazyConstructAndRegisterSingleton<IPhotoPropertiesService, PhotoPropertiesService>();
        }

        public static IBingSpeech Change;
        public static void Init(IBingSpeech _change)
        {
            Change = _change;
        }

    }
}
