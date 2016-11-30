using MvvmCross.Core.ViewModels;

namespace CognitiveServicesDemo.ViewModels
{
    public class MainViewModel : MvxViewModel
    {
        public MainViewModel()
        {
                
        }

        public override void Start()
        {
            base.Start();
        }

        public void ShowEmotion()
        {
            ShowViewModel<VisionEmotionViewModel>();
        }

        public void ShowFace()
        {
            ShowViewModel<VisionFaceViewModel>();
        }

        public void ShowBingSpellCheck()
        {
            ShowViewModel<LanguageBingSpellCheckViewModel>();
        }

        public void ShowTextAnalytics()
        {
            ShowViewModel<LanguageTextAnalyticsViewModel>();
        }

        public void ShowWlm()
        {
            ShowViewModel<LanguageWebLanguageModelViewModel>();
        }

        public void ShowEntityLinking()
        {
            ShowViewModel<EntityLinkingViewModel>();
        }

        public void ShowBingAutosuggest()
        {
            ShowViewModel<SearchBingAutosuggestViewModel>();
        }

        public void ShowBingWebSearch()
        {
            ShowViewModel<BingWebSearchViewModel>();
        }

        public void ShowBingSpeech()
        {
            ShowViewModel<BingSpeechViewModel>();
        }

        public void ShowBingImageSearch()
        {
            ShowViewModel<BingImageSearchViewModel>();
        }

        public void ShowAcademic()
        {
            ShowViewModel<AcademicViewModel>();
        }
    }
}
