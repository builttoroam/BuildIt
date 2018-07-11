using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BuildIt.Forms.Controls
{
    /// <summary>
    /// Represents a ContentView which can be used to display a live preview of a device's camera
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CameraPreviewControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CameraPreviewControl"/> class.
        /// </summary>
        public CameraPreviewControl()
        {
            InitializeComponent();
        }

        ///// <summary>
        ///// Gets the <see cref="Image"/> used for rendering the camera preview
        ///// </summary>
        //internal Image PreviewImage => CameraPreviewImage;
    }
}