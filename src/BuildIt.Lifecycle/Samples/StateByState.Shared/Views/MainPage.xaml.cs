﻿using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Diagnostics;
using Windows.UI.Xaml;
using BuildIt.Lifecycle;
using System;
using Windows.UI.Xaml.Controls;
using BuildIt.General.UI;
using BuildIt.States;
using StateTriggerBase = BuildIt.States.StateTriggerBase;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace StateByState
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage: ICodeBehindViewModel<MainViewModel>

    {
        public enum TestStates
        {
            Base,
            Custom
        }

        private StateManager sm=new StateManager();

        public MainPage()
        {
            Data = new ContextWrapper<MainViewModel>(this);
            InitializeComponent();

            sm.Group<TestStates>()
                .DefineState(TestStates.Custom)
                .AddTrigger(new WindowSizeTrigger(this) {MinWidth = 700});

            (sm.StateGroups[typeof(TestStates)] as IStateGroup<TestStates>).StateChanged += MainPage_StateChanged;
        }

        private void MainPage_StateChanged(object sender, StateEventArgs<TestStates> e)
        {
            Debug.WriteLine($"State: {e.State}");
        }

        public MainViewModel CurrentViewModel => DataContext as MainViewModel;

        public ContextWrapper<MainViewModel> Data { get; }

        private void RegularCodebehindHandler(object sender, RoutedEventArgs e)
        {
           // Debug.WriteLine(ViewModel != null);
        }

        private void Test(object sender, RoutedEventArgs e)
        {
            Data.ViewModel.Test();
        }

        private void Three(object sender, RoutedEventArgs e)
        {
            Data.ViewModel.Three();
        }

        private void Fourth(object sender, RoutedEventArgs e)
        {
            Data.ViewModel.Fourth();
        }

        private void Spawn(object sender, RoutedEventArgs e)
        {
            Data.ViewModel.Spawn();
        }
    }


    public class WindowSizeTrigger : StateTriggerBase
    {

        public int MinWidth { get; set; }   

       
        public WindowSizeTrigger(Page page)
        {
            page.SizeChanged += Page_SizeChanged;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateIsActive(e.NewSize.Width>=MinWidth);
        }
    }

}