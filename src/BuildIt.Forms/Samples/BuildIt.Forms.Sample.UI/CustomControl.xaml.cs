using BuildIt.forms.Sample.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BuildIt.Forms.Sample
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CustomControl : ContentView
	{
        public CustomViewModel ViewModel => BindingContext as CustomViewModel;
        public CustomControl ()
		{
			InitializeComponent ();

            BindingContext = new CustomViewModel();

        }

        public void ShowClicked(object sender, EventArgs e)
        {
            ViewModel.SwitchStates(true);
            //VisualStateManager.GoToState(this, "Show");
        }

        public void HideClicked(object sender, EventArgs e)
        {
            ViewModel.SwitchStates(false);
            //VisualStateManager.GoToState(this, "Hide");
        }

        public async Task BindVisualStates()
        {
            await VisualStateManager.Bind(this, ViewModel.StateManager);
        }
    }
}