using BuildIt.Forms.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using BuildIt.Forms.Controls;

namespace BuildIt.Forms.Sample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            //var x = new Label();
            //x.TextColor

            //var x = new Button();
            //x.TextColor

            //var x = new Frame();
            //x.BackgroundColor;

            //var x = new BoxView();
            //x.Color;

            //var cv = new ContentView();
            //cv.BackgroundColor;
        }

        private bool visible = true;
        public void ToggleButtonPressed(object sender, EventArgs e)
        {
            visible = !visible;
            VisualStateManager.GoToState(this, visible ? "Show":"Hide");
        }

        public void ButtonClicked(object sender, EventArgs e)
        {
            DisabledButton.IsEnabled = !DisabledButton.IsEnabled;
        }
    }
}
