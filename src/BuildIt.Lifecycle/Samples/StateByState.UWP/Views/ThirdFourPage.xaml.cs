using StateByState.Regions.Main;

namespace StateByState
{
    public sealed partial class ThirdFourPage
    {
        public ThirdFourPage()
        {
            InitializeComponent();
        }

        public ThirdFourViewModel CurrentViewModel => DataContext as ThirdFourViewModel;
    }
}