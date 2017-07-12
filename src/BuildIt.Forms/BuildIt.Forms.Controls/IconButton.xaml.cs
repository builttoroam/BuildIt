using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BuildIt.Forms.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IconButton
    {
        public static readonly BindableProperty IconProperty = BindableProperty.Create("Icon", typeof(string), typeof(IconButton), default(string));

        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public IconButton()
        {
            InitializeComponent();

            IconLabel.BindingContext = this;
        }
    }
}