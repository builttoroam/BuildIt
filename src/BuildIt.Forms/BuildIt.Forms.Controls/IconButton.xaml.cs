using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BuildIt.Forms.Controls
{
    /// <summary>
    /// Icon button control.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IconButton
    {
        /// <summary>
        /// The icon to use.
        /// </summary>
        public static readonly BindableProperty IconProperty =
            BindableProperty.Create("Icon", typeof(string), typeof(IconButton), default(string));

        /// <summary>
        /// Initializes a new instance of the <see cref="IconButton"/> class.
        /// </summary>
        public IconButton()
        {
            InitializeComponent();

            IconLabel.BindingContext = this;
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
    }
}