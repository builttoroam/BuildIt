using BuildIt.forms.Sample.Core.ViewModels;
using BuildIt.States.Interfaces;
using System;
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

        public async void ToggleClicked(object sender, EventArgs e)
        {
            var vm = BindingContext as ItemViewModel;
            await vm.Toggle();
        }
    }
}