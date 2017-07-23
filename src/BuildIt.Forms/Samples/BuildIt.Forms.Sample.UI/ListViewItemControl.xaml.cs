using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildIt.Forms;
using BuildIt.States;
using BuildIt.States.Interfaces;
using BuildIt.forms.Sample.Core.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BuildIt.Forms.Sample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListViewItemControl : ContentView
    {
        public ListViewItemControl()
        {
            InitializeComponent();
        }

        private IStateBinder Binder { get; set; }

        protected override async void OnBindingContextChanged()
        {
            Binder?.Dispose();

            base.OnBindingContextChanged();

            if (BindingContext is ItemViewModel itemViewModel)
            {
                Binder = await VisualStateManager.Bind(this.Content, itemViewModel?.StateManager);
            }
        }
    }
}