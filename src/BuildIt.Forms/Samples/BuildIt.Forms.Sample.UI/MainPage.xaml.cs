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
