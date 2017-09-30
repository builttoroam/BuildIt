namespace StateByState.XForms.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new StateByState.XForms.App());
        }
    }
}