using StateByState.Regions.Main;
using StateByState.Regions.Main.Third;

namespace StateByState.Views.Third
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