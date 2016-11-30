using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Xamarin.Forms;
using CognitiveServicesDemo.ViewModels;

namespace CognitiveServicesDemo.Views
{
    public partial class VisionEmotion : ContentPage
    {
        public VisionEmotionViewModel CurrentViewModel => BindingContext as VisionEmotionViewModel;
        public FaceServiceClient FaceServiceClient { get; set; }
        
        public VisionEmotion()
        {
            InitializeComponent();
        }
    }
}
