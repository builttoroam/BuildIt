using Xamarin.Forms;
using CognitiveServicesDemo.ViewModels;

namespace CognitiveServicesDemo.Views
{
    public partial class VisionEmotion : ContentPage
    {
        public VisionEmotionViewModel CurrentViewModel => BindingContext as VisionEmotionViewModel;
        //public FaceServiceClient FaceServiceClient { get; set; }
        
        public VisionEmotion()
        {
            InitializeComponent();
        }
    }
}
