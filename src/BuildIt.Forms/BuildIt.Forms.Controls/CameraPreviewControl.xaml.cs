using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BuildIt.Forms.Controls
{
    /// <summary>
    /// A simple controled inheritng from <see cref="Frame"/> which shows a live preview of the camera. Defaults to rear/first-available camera
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CameraPreviewControl
    {
        /// <summary>
        /// Informs the previewer as to which camera on the device should be used (if available)
        /// </summary>
        public static readonly BindableProperty PreferredCameraProperty =
            BindableProperty.Create(nameof(PreferredCamera), typeof(CameraPreference), typeof(CameraPreviewControl), CameraPreference.Back);

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraPreviewControl"/> class.
        /// </summary>
        public CameraPreviewControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Enumeration specifying which camera should be used
        /// </summary>
        public enum CameraPreference
        {
            /// <summary>
            /// Prefer the camera located on the back of the device enclosure
            /// </summary>
            Back,

            /// <summary>
            /// Prefer the front-facing camera
            /// </summary>
            Front
        }

        /// <summary>
        /// Gets or sets the preferred camera to be used for the preview
        /// </summary>
        public CameraPreference PreferredCamera
        {
            get => (CameraPreference)GetValue(PreferredCameraProperty);
            set => SetValue(PreferredCameraProperty, value);
        }
    }
}