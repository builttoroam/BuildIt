using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildIt.Forms;
using BuildIt.States;
using BuildIt.States.Interfaces;
using BuildIt.Forms.Core;
using BuildIt.forms.Sample.Core.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BuildIt.Forms.Sample
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ListViewItemControl : ContentView
	{
		public ListViewItemControl ()
		{
			InitializeComponent ();

            BindingContextChanged += ContextChanged;
		}

        public async void ContextChanged(object sender, EventArgs e)
        {
            if (BindingContext is ItemViewModel)
            {
                VisualStateManager.Bind(this, (BindingContext as IHasStates)?.StateManager);

                //await (BindingContext as ItemViewModel).Init();
            }
        }
    }
}