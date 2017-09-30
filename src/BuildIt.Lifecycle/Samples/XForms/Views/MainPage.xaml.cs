using System;

namespace StateByState.XForms.Views
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public void TestClick(object sender, EventArgs e)
        {
            (BindingContext as MainViewModel).Test();
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            (BindingContext as MainViewModel).Test();
        }
    }
}