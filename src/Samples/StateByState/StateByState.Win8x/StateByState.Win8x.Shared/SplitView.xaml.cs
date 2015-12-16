using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace StateByState
{
    public sealed partial class CustomSplitView 
    {


        public UIElement Pane
        {
            get { return (UIElement)GetValue(PaneProperty); }
            set { SetValue(PaneProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Pane.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PaneProperty =
            DependencyProperty.Register("Pane", typeof(UIElement), typeof(CustomSplitView), new PropertyMetadata(null,PaneChanged));

        private static void PaneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sv = (d as CustomSplitView);
            sv.SplitPane.Content = sv.Pane;
        }



        public UIElement Content
        {
            get { return (UIElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(UIElement), typeof(CustomSplitView), new PropertyMetadata(null,ContentChanged));

        private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sv = (d as CustomSplitView);
            sv.SplitContent.Content = sv.Content;
        }


        public string DisplayMode { get; set; }

        public string IsPaneOpen { get; set; }

        public double OpenPaneLength { get; set; }


        public CustomSplitView()
        {
            this.InitializeComponent();
        }
    }
}
