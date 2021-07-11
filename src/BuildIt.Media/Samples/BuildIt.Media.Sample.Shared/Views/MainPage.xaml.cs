using System;
#pragma warning disable SA1005 // Single line comments should begin with single space
#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#endif
using Windows.UI.Popups;
#pragma warning restore SA1005 // Single line comments should begin with single space

namespace BuildIt.Media.Sample.Views
{
#pragma warning disable CA1010 // Generic interface should also be implemented
    public sealed partial class MainPage
#pragma warning restore CA1010 // Generic interface should also be implemented
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public async void StartPreviewClick(object sender, RoutedEventArgs args)
        {
            try
            {

                await preview.StartPreview();

            }
            catch(Exception ex)
            {
                var dlg = new MessageDialog(ex.Message + "-" + ex.StackTrace, "Error");
                dlg.ShowAsync();
            }
        }
    }
}
