
namespace StateByState
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SecondPage
    {
        public SecondPage()
        {
            InitializeComponent();
        }

        public SecondViewModel CurrentViewModel =>DataContext as SecondViewModel;

    }
}
