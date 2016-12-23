using MvvmCross.Core.ViewModels;

namespace CognitiveServicesDemo.ViewModels
{
    public class MainViewModel : MvxViewModel
    {
        public IMvxCommand ShowEmotionCommand { get; }
        public IMvxCommand ShowFaceCommand { get; }
        public IMvxCommand ShowBindSpellCheckCommand { get; }
        public IMvxCommand ShowTextAnalyticsCommand { get; }
        public IMvxCommand ShowWlmCommand { get; }
        public IMvxCommand ShowEentityLinkingCommand { get; }
        public IMvxCommand ShowBingAutoSuggestCommand { get; }
        public IMvxCommand ShowBingWebSearchCommand { get; }
        public IMvxCommand ShowBingSpeechCommand { get; }
        public IMvxCommand ShowBingImageSearchCommand { get; }
        public IMvxCommand ShowAcademicCommand { get; }
        public IMvxCommand ShowVideoCommand { get; }

        public MainViewModel()
        {
            ShowEmotionCommand = new MvxCommand(ShowEmotion);
            ShowFaceCommand = new MvxCommand(ShowFace);
            ShowBindSpellCheckCommand = new MvxCommand(ShowBingSpellCheck);
            ShowTextAnalyticsCommand = new MvxCommand(ShowTextAnalytics);
            ShowWlmCommand = new MvxCommand(ShowWlm);
            ShowEentityLinkingCommand = new MvxCommand(ShowEntityLinking);
            ShowBingAutoSuggestCommand = new MvxCommand(ShowBingAutosuggest);
            ShowBingWebSearchCommand = new MvxCommand(ShowBingWebSearch);
            ShowBingSpeechCommand = new MvxCommand(ShowBingSpeech);
            ShowBingImageSearchCommand = new MvxCommand(ShowBingImageSearch);
            ShowAcademicCommand = new MvxCommand(ShowAcademic);
            ShowVideoCommand = new MvxCommand(ShowVideo);
        }

        private void ShowEmotion()
        {
            ShowViewModel<VisionEmotionViewModel>();
        }

        private void ShowFace()
        {
            ShowViewModel<VisionFaceViewModel>();
        }

        private void ShowBingSpellCheck()
        {
            ShowViewModel<LanguageBingSpellCheckViewModel>();
        }

        public void ShowTextAnalytics()
        {
            ShowViewModel<LanguageTextAnalyticsViewModel>();
        }

        private void ShowWlm()
        {
            ShowViewModel<LanguageWebLanguageModelViewModel>();
        }

        private void ShowEntityLinking()
        {
            ShowViewModel<EntityLinkingViewModel>();
        }

        private void ShowBingAutosuggest()
        {
            ShowViewModel<SearchBingAutosuggestViewModel>();
        }

        private void ShowBingWebSearch()
        {
            ShowViewModel<BingWebSearchViewModel>();
        }

        private void ShowBingSpeech()
        {
            ShowViewModel<BingSpeechViewModel>();
        }

        private void ShowBingImageSearch()
        {
            ShowViewModel<BingImageSearchViewModel>();
        }

        private void ShowAcademic()
        {
            ShowViewModel<AcademicViewModel>();
        }

        private void ShowVideo()
        {
            ShowViewModel<VisionVideoFacialRecognitionViewModel>();
        }
    }
}
