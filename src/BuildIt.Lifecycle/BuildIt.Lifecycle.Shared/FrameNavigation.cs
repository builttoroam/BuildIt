using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BuildIt.Lifecycle.States;
using BuildIt.Lifecycle.States.ViewModel;
using BuildIt.States;
using BuildIt.States.Interfaces;

namespace BuildIt.Lifecycle
{
   

    public class FrameNavigation<TState>: IHasStateData
        where TState : struct
    {
        public INotifyPropertyChanged CurrentStateData => (StateNotifier as IHasStateData) ?.CurrentStateData;


        public INotifyEnumStateChanged<TState> StateNotifier { get; }

        private Frame RootFrame { get; }

        public FrameNavigation(Frame rootFrame,
            INotifyEnumStateChanged<TState> stateNotifier)
            //,string registerAs = null)
        {
            //var stateManager = hasStateManager.StateManager;
            //if (string.IsNullOrWhiteSpace( registerAs ))
            //{
            //    registerAs = hasStateManager.GetType().Name;
            //}
            //Application.Current.Resources[registerAs] = this;
            RootFrame = rootFrame;

            RootFrame.Navigated += RootFrame_Navigated;
            RootFrame.Navigating += RootFrame_Navigating;

            //RootFrame.Tag = registerAs;
            StateNotifier = stateNotifier;
            StateNotifier.EnumStateChanged += StateManager_StateChanged;
        }

        private void RootFrame_Navigating(object sender, Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e)
        {
        }

        private void RootFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var dc = e.Parameter as INotifyPropertyChanged;
            $"Found data context? '{dc != null}'".Log();
            if (dc == null) return;


            var pg = RootFrame.Content as FrameworkElement;
            if (pg == null) return;

            pg.DataContext = dc;

            //var pgHasNotifier = pg as IHasStates;
            //if (pgHasNotifier == null) return;


            var sm = (dc as IHasStates)?.StateManager;
            if (sm != null)
            {

                var groups = sm.StateGroups;
                var inotifier = typeof (INotifyEnumStateChanged<>);
                var vsct = typeof (VisualStateChanger<>);
                foreach (var stateGroup in groups)
                {
                    var groupNotifier = inotifier.MakeGenericType(stateGroup.Key);
                    if (stateGroup.Value.GetType().GetTypeInfo().ImplementedInterfaces.Contains(groupNotifier))
                    {
                        var vsc = Activator.CreateInstance(vsct.MakeGenericType(stateGroup.Key), pg, stateGroup.Value);
                    }
                }
            }
            //var pps = dc.GetType().GetTypeInfo().DeclaredProperties;
            //"Iterating through declared properties".Log();
            //foreach (var p in pps)
            //{
            //    var pt = p.PropertyType.GetTypeInfo();
            //    var interfaces = pt.ImplementedInterfaces;
            //    if (pt.IsInterface)
            //    {
            //        interfaces = new[] { pt.AsType() }.Union(interfaces);
            //    }
            //    "Completed interface search".Log();
            //    var ism = typeof(IStateManager<,>);
            //    var vsct = typeof(VisualStateChanger<,>);
            //    foreach (var inf in interfaces)
            //    {
            //        $"Inspecting interface {inf.Name}".Log();
            //        if (inf.IsConstructedGenericType &&
            //            inf.GetGenericTypeDefinition() == ism)
            //        {
            //            "Interface matched, creating instance".Log();
            //            var parm = inf.GenericTypeArguments;
            //            var vsc = Activator.CreateInstance(vsct.MakeGenericType(parm), pg, p.GetValue(dc));
            //            "Instance created".Log();
            //        }
            //    }
            //}


        }

        private void StateManager_StateChanged(object sender, EnumStateEventArgs<TState> e)
        {
            var tp = NavigationHelper.TypeForState(e.EnumState);

            if (e.IsNewState)
            {
                RootFrame.Navigate(tp, CurrentStateData);
            }
            else
            {
                var previous = RootFrame.BackStack.FirstOrDefault();
                while (previous != null && previous.SourcePageType != tp)
                {
                    RootFrame.BackStack.Remove(previous);
                    previous = RootFrame.BackStack.FirstOrDefault();
                }
                if (previous != null)
                {
                    RootFrame.GoBack();
                }
            }
        }


    }
}